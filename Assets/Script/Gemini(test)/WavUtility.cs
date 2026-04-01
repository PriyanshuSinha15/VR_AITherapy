using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        MemoryStream stream = new MemoryStream();

        int samples = clip.samples * clip.channels;
        float[] data = new float[samples];
        clip.GetData(data, 0);

        short[] intData = new short[samples];
        byte[] bytesData = new byte[samples * 2];

        const float rescale = 32767;

        for (int i = 0; i < samples; i++)
        {
            intData[i] = (short)(data[i] * rescale);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        // WAV HEADER
        WriteHeader(stream, clip, bytesData.Length);
        stream.Write(bytesData, 0, bytesData.Length);

        return stream.ToArray();
    }

    static void WriteHeader(Stream stream, AudioClip clip, int dataLength)
    {
        int hz = clip.frequency;
        int channels = clip.channels;

        BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(36 + dataLength);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)channels);
        writer.Write(hz);
        writer.Write(hz * channels * 2);
        writer.Write((short)(channels * 2));
        writer.Write((short)16);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write(dataLength);
    }
}