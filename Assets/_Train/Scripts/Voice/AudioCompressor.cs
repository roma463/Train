using System.IO.Compression;
using System.IO;
using UnityEngine;
using System;

public class AudioCompressor : MonoBehaviour
{
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

    public static byte[] FloatToByte(float[] samples)
    {
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * 32767);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        return bytesData;
    }
}
