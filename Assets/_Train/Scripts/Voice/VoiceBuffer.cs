using System;
using UnityEditor;
using UnityEngine;

public class VoiceBuffer
{

    private int _curIndex = 0;
    private int _endIndex = 0;

    private float[] _clipBuffer = new float[Settings.sampleRate];
    private float[] _buffer = new float[320000];
    private int _sampleRate => Settings.sampleRate;

    #region Constructors

    public VoiceBuffer()
    {
        _curIndex = _endIndex = 0;
        _clipBuffer = new float[Settings.sampleRate];
        _buffer = new float[(Settings.sampleRate * 4) * 10];
    }

    public VoiceBuffer(out AudioClip clip)
    {
        _curIndex = _endIndex = 0;
        _clipBuffer = new float[Settings.sampleRate];
        _buffer = new float[(Settings.sampleRate * 4) * 10];
        clip = AudioClip.Create("BufferedClip_" + Time.time, Settings.sampleRate * Settings.audioClipDuration, 1,
            Settings.sampleRate, false);
    }

    #endregion

    public void Add(byte[] voiceData)
    {
        float[] voice = ByteToFloat(voiceData);
        int dataLeft = voice.Length;

        while (dataLeft > 0)
        {
            if (_endIndex + dataLeft >= _buffer.Length)
            {
                int oversize = (_endIndex + dataLeft) - _buffer.Length;
                Array.Copy(voice, voice.Length - dataLeft, _buffer, _endIndex, dataLeft - oversize);
                dataLeft -= oversize;
                _endIndex = 0;
            }
            else
            {
                Array.Copy(voice, voice.Length - dataLeft, _buffer, _endIndex, dataLeft);
                _endIndex += dataLeft;
                break;
            }
        }
    }

    public void Clear()
    {
        _curIndex = _endIndex = 0;
        Array.Clear(_buffer, 0, _buffer.Length);
        Array.Clear(_clipBuffer, 0, _clipBuffer.Length);
    }

    public bool NextVoice_IsReady()
    {
        return _curIndex != _endIndex;
    }

    public bool NextVoice_TryWrite(ref AudioClip clip, out float playTime)
    {
        if (TryWriteNextClipBuffer(out playTime))
        {
            clip.SetData(_clipBuffer, 0);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool NextVoice_GetData(out float[] audio, out float playTime)
    {
        if (TryWriteNextClipBuffer(out playTime))
        {
            audio = _clipBuffer;
            return true;
        }
        else
        {
            audio = null;
            return false;
        }
    }

    private bool TryWriteNextClipBuffer(out float playTime)
    {
        if (_curIndex == _endIndex)
        {
            playTime = 0f;
            return false;
        }

        Array.Clear(_clipBuffer, 0, _clipBuffer.Length);

        int copyIndex = 0;
        int dataLeft = _curIndex < _endIndex ? _endIndex - _curIndex : (_buffer.Length - _curIndex) + _endIndex;
        dataLeft = Mathf.Clamp(dataLeft, 0, _clipBuffer.Length);

        playTime = ((float)dataLeft / (float)_sampleRate);

        while (dataLeft > 0)
        {
            if (_curIndex + dataLeft >= _buffer.Length)
            {
                int oversize = (_curIndex + dataLeft) - _buffer.Length;
                Array.Copy(_buffer, _curIndex, _clipBuffer, copyIndex, dataLeft - oversize);
                dataLeft -= oversize;
                copyIndex += oversize;
                _curIndex = 0;
            }
            else
            {
                Array.Copy(_buffer, _curIndex, _clipBuffer, copyIndex, dataLeft);
                _curIndex += dataLeft;
                break;
            }
        }

        return true;
    }

    public static float[] ByteToFloat(byte[] data)
    {
        int length = data.Length / 2;
        float[] samples = new float[length];
        for (int i = 0; i < length; i++)
            samples[i] = (float)BitConverter.ToInt16(data, i * 2) / 32767;
        return samples;
    }
}

public static class Settings
{

    /// <summary>
    /// If TRUE, than the recorder will only send data when the voice level is high enough. If FALSE, it will send data constantly.
    /// </summary>
    public const bool autoVoiceDetection = false;

    /// <summary>
    /// Threshold for auto voice detection.
    /// </summary>
    public const float voiceDetectionThreshold = 0.02f;

    /// <summary>
    /// Should we compress audio stream before sending via network?
    /// This value should be the same for the listener and speaker.
    /// </summary>
    public const bool compression = true;

    /// <summary>
    /// Piece time (milliseconds)
    /// </summary>
    public const int pieceDuration = 150;

    /// <summary>
    /// The sampling rate used for audio recording and playback (8000, 16000, 32000).
    /// Make this value smaller when you have troubles sending big values via network.
    /// </summary>
    public const int sampleRate = 8000;

    /// <summary>
    /// Size of data which is sent via network.
    /// </summary>
    public const int pieceSize = (int)(sampleRate * ((float)pieceDuration / 1000f)); // send with interval ~ pieceDuration ms

    /// <summary>
    /// What is size of audio clip, used by microphone (seconds). Audio clip is looped and rewritten from beginning when overflowed.
    /// </summary>
    public const int audioClipDuration = 1;
}
