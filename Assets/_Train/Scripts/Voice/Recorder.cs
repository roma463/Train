using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Mirror;

/// <summary>
/// Place prefab with this script to scene to be able record voice audio. Make sure there is only one 'Recorder' in the scene.
/// </summary>
public class Recorder : NetworkBehaviour
{
    /// <summary>
    /// It is called when recording is started successfully.
    /// </summary>
    public static event Action OnRecordingStart;

    /// <summary>
    /// It is called when recording was stopped.
    /// </summary>
    public static event Action OnRecordingEnd;

    /// <summary>
    /// It is called when recording is fail by some reason.
    /// </summary>
    public static event Action<string> OnRecordingFail;

    /// <summary>
    /// It is called when 'Recorded' is ready to send piece of voice data to network.
    /// Subscribe to this event and implement actual network transfer.
    /// </summary>
    public static event Action<byte[]> OnSendDataToNetwork;

    /// <summary>
    /// Last cached position in samples data.
    /// </summary>
    private int _lastPosition = 0;

    /// <summary>
    /// Last position of microphone when recording is stopped.
    /// </summary>
    private int _stopRecordPosition = -1;

    /// <summary>
    /// A buffer for recorded audio data.
    /// </summary>
    private List<float> _buffer;

    /// <summary>
    /// Audio clip used for recording audio from microphone.
    /// </summary>
    private AudioClip _workingClip;

    /// <summary>
    /// Current selected microphone device in usage.
    /// </summary>
    private string _currentMicrophone;

    /// <summary>
    /// Raw audio data from microphone.
    /// </summary>
    private float[] _rawSamples;

    /// <summary>
    /// Average audio level. It is used for auto voice detecting (when enabled in Settings).
    /// </summary>
    private float _averageVoiceLevel = 0f;

    /// <summary>
    /// Set this to TRUE for temporary stop sending audio.
    /// </summary>
    [Tooltip("You can change this value from Inspector or via script to temporary disable speaking.")]
    public bool IsMuted = false;

    /// <summary>
    /// Is recording in progress now?
    /// </summary>
    [Tooltip("Don't change this value in Inspector. Use methods 'Start_Record' and 'Stop_Record' instead.")]
    public bool IsRecording = false;

    [Header("Debug")]
    [Tooltip("Do we need to play the voice from the microphone back on this device? If yes, then make sure that the 'echo speaker' is set.")]
    [SerializeField] private bool debugEcho = false;
    [Tooltip("It is used to debug play voice back. Feel free to make it null if you don't need this feature.")]
    [SerializeField] private Speaker echoSpeaker;

    private INPUTE _inpute;

    private void OnDestroy()
    {
        if (!isLocalPlayer || _inpute == null)
            return;

        _inpute.OnPerformedR -= SwitchState;
    }

    /// <summary>
    /// Initializes buffer, refreshes microphones list and selects first microphone device if exists
    /// </summary>
    void Start()
    {
        if (!isLocalPlayer)
            return;

        _inpute = INPUTE.instance;
        _inpute.OnPerformedR += SwitchState;

        _buffer = new List<float>();
        _rawSamples = new float[Settings.audioClipDuration * Settings.sampleRate];

        if (HasConnectedMicrophoneDevices() && string.IsNullOrEmpty(_currentMicrophone))
        {
            _currentMicrophone = Microphone.devices[0];
        }
        else
            Debug.Log($"[Recorder] Can't set any microphone.", gameObject);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!string.IsNullOrEmpty(_currentMicrophone))
        {
            ProcessRecording();
        }
    }

    public bool StartRecord()
    {
        if (string.IsNullOrEmpty(_currentMicrophone))
        {
            OnRecordingFail?.Invoke("[Recorder] Can't start recording. No microphone selected.");
            return false;
        }
        if (IsRecording)
        {
            OnRecordingFail?.Invoke("Recording is in progress. Can't start new one.");
            return false;
        }

        _stopRecordPosition = -1;
        _buffer?.Clear();
        _workingClip = Microphone.Start(Microphone.devices[0], true, 1, Settings.sampleRate);
        IsRecording = true;
        OnRecordingStart?.Invoke();
        return true;
    }

    public bool StopRecord()
    {
        if (!IsRecording)
            return false;
        _stopRecordPosition = Microphone.GetPosition(_currentMicrophone);

        if (_workingClip != null)
        {
            Destroy(_workingClip);
        }

        IsRecording = false;
        OnRecordingEnd?.Invoke();
        return true;
    }

    public void SwitchState()
    {
        if (IsRecording)
            StopRecord();
        else
            StartRecord();
    }

    private void ProcessRecording()
    {
        int currentPosition = Microphone.GetPosition(_currentMicrophone);

        if (_stopRecordPosition != -1)
            currentPosition = _stopRecordPosition;

        if ((IsRecording || currentPosition != _lastPosition) && !IsMuted)
        {
            {
                _workingClip.GetData(_rawSamples, 0);
                if (_lastPosition != currentPosition && _rawSamples.Length > 0)
                {
                    if (!Settings.autoVoiceDetection || VoiceIsDetected(_rawSamples))
                    {
                        if (_lastPosition > currentPosition)
                        {
                            _buffer.AddRange(GetPieceOfData(_rawSamples, _lastPosition, _rawSamples.Length - _lastPosition));
                            _buffer.AddRange(GetPieceOfData(_rawSamples, 0, currentPosition));
                        }
                        else
                        {
                            _buffer.AddRange(GetPieceOfData(_rawSamples, _lastPosition, currentPosition - _lastPosition));
                        }
                    }
                    if (_buffer.Count >= Settings.pieceSize)
                    {
                        PrepareDataForTransfer(_buffer.GetRange(0, Settings.pieceSize));
                        _buffer.RemoveRange(0, Settings.pieceSize);
                    }
                }
            }
            _lastPosition = currentPosition;
        }
        else
        {
            _lastPosition = currentPosition;

            if (_buffer.Count > 0)
            {
                if (_buffer.Count >= Settings.pieceSize)
                {
                    PrepareDataForTransfer(_buffer.GetRange(0, Settings.pieceSize));
                    _buffer.RemoveRange(0, Settings.pieceSize);
                }
                else
                {
                    PrepareDataForTransfer(_buffer);
                    _buffer.Clear();
                }
            }
        }
    }

    private float[] GetPieceOfData(float[] data, int startIndex, int length)
    {
        if (data.Length < startIndex + length)
            throw new Exception("Wrong length when getting piece of data.");
        float[] output = new float[length];
        Array.Copy(data, startIndex, output, 0, length);
        return output;
    }

    private void PrepareDataForTransfer(List<float> samples)
    {
        byte[] bytes = AudioCompressor.FloatToByte(samples.ToArray());
        if (Settings.compression)
            bytes = AudioCompressor.Compress(bytes);

        var maxSize = NetworkMessages.MaxMessageSize(Channels.Unreliable);
// Резервируем место для служебных данных (обычно 10-50 байт)
        int headerSize = 20; // Настройте это значение в зависимости от вашей реализации
        int availablePayloadSize = maxSize - headerSize;

        Debug.Log($"[Recorder] Max size: {maxSize}, Available for payload: {availablePayloadSize}");

        if (bytes.Length > availablePayloadSize)
        {
            int totalBytes = bytes.Length;
            int bytesSent = 0;
            int chunkCount = 0;
    
            while (bytesSent < totalBytes)
            {
                int bytesToSend = Math.Min(availablePayloadSize, totalBytes - bytesSent);
                byte[] chunk = new byte[bytesToSend];
        
                Buffer.BlockCopy(bytes, bytesSent, chunk, 0, bytesToSend);
        
                Debug.Log($"Sending chunk {++chunkCount}, size: {bytesToSend}");
                SendToNetwork(chunk);
        
                bytesSent += bytesToSend;
            }
    
            Debug.Log($"Sent {chunkCount} chunks total");
        }
        else
        {
            SendToNetwork(bytes);
        }
    }

    [Command( channel = Channels.Unreliable)]
    public virtual void SendToNetwork(byte[] bytes)
    {
        RpcReceiveVoiceData(bytes);

        OnSendDataToNetwork?.Invoke(bytes);
    }

    [ClientRpc(channel = Channels.Unreliable, includeOwner = false)]
    private void RpcReceiveVoiceData(byte[] compressedData)
    {
        echoSpeaker.ProcessVoiceData(compressedData);
    }

    public void SetMicrophone(string microphone)
    {
        if (!string.IsNullOrEmpty(_currentMicrophone))
            Microphone.End(_currentMicrophone);
        _currentMicrophone = microphone;
    }

    bool HasConnectedMicrophoneDevices()
    {
        return Microphone.devices.Length > 0;
    }

    bool VoiceIsDetected(float[] samples)
    {
        bool detected = false;
        double sumTwo = 0;
        double tempValue;

        for (int index = 0; index < samples.Length; index++)
        {
            tempValue = samples[index];
            sumTwo += tempValue * tempValue;
            if (tempValue > Settings.voiceDetectionThreshold)
                detected = true;
        }
        sumTwo /= samples.Length;
        _averageVoiceLevel = (_averageVoiceLevel + (float)sumTwo) / 2f;

        return detected || sumTwo > Settings.voiceDetectionThreshold;
    }
}
