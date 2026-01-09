using System.IO.Compression;
using System.IO;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    private VoiceBuffer _buffer;
    private AudioSource _source;
    private AudioClip _voiceClip;
    private float _testDelay;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        if (_testDelay == 0f)
        {
            if (_buffer.NextVoice_IsReady() && _buffer.NextVoice_TryWrite(ref _voiceClip, out _testDelay))
            {
                _source.Play();
            }
        }
        else if ((_testDelay -= Time.deltaTime) <= 0f)
        {
            _source.Stop();
            _testDelay = 0f;
        }
    }

    public Speaker Initialize()
    {
        gameObject.name = "Voice speaker";
        if (_source == null)
            _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();
        _buffer = new VoiceBuffer(out var clip);
        _source.clip = _voiceClip = clip;
        return this;
    }

    /// <summary>
    /// Direct audio stream from the network to this method.
    /// </summary>
    public void ProcessVoiceData(byte[] voiceData)
    {
        _buffer.Add(Settings.compression ? Decompress(voiceData) : voiceData);
    }

    public static byte[] Compress(byte[] data)
    {
        MemoryStream memStream = new MemoryStream();
        using (GZipStream gZip = new GZipStream(memStream, System.IO.Compression.CompressionLevel.Optimal))
        {
            gZip.Write(data, 0, data.Length);
        }
        return memStream.ToArray();
    }

    public static byte[] Decompress(byte[] data)
    {
        MemoryStream inputStream = new MemoryStream(data);
        MemoryStream resultStream = new MemoryStream();
        using (GZipStream gZip = new GZipStream(inputStream, CompressionMode.Decompress))
        {
            gZip.CopyTo(resultStream);
        }
        return resultStream.ToArray();
    }
}
