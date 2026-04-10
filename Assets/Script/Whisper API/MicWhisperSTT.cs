using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class MicWhisperSTT : MonoBehaviour
{
    [Header("OpenAI")]
    public string apiKey = "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";

    [Header("API Manager Script")]
    public ApiManager apiManager;
    [Space]
    private AudioClip recordedClip;
    private string filePath;

    // 🎤 Start Recording
    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, 10, 16000);
        Debug.Log("Recording started...");
    }

    // 🛑 Stop Recording & Send
    public void StopRecording()
    {
        Microphone.End(null);
        Debug.Log("Recording stopped");

        filePath = Path.Combine(Application.persistentDataPath, "recorded.wav");
        SaveWav(filePath, recordedClip);

        StartCoroutine(SendAudio(filePath));
    }

    // 📤 Upload to OpenAI
    IEnumerator SendAudio(string path)
    {
        byte[] audioData = File.ReadAllBytes(path);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recorded.wav");
        form.AddField("model", "gpt-4o-mini-transcribe");

        UnityWebRequest request = UnityWebRequest.Post(
            "https://api.openai.com/v1/audio/transcriptions",
            form
        );

        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Transcription: " + request.downloadHandler.text);
            
            // Sending data from whisper to backend
            var json = JObject.Parse(request.downloadHandler.text);
            string transcribedText = json["text"].ToString();

            apiManager.message = transcribedText;
            apiManager.StartStreaming();
        }
    }

    // 💾 Convert AudioClip → WAV
    void SaveWav(string filename, AudioClip clip)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filename));

        using (FileStream fileStream = new FileStream(filename, FileMode.Create))
        {
            int headerSize = 44;
            float[] samples = new float[clip.samples];
            clip.GetData(samples, 0);

            short[] intData = new short[samples.Length];
            byte[] bytesData = new byte[samples.Length * 2];

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * 32767);
                byte[] byteArr = System.BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            // WAV Header
            byte[] header = new byte[headerSize];

            System.Text.Encoding.ASCII.GetBytes("RIFF").CopyTo(header, 0);
            System.BitConverter.GetBytes(bytesData.Length + 36).CopyTo(header, 4);
            System.Text.Encoding.ASCII.GetBytes("WAVE").CopyTo(header, 8);
            System.Text.Encoding.ASCII.GetBytes("fmt ").CopyTo(header, 12);
            System.BitConverter.GetBytes(16).CopyTo(header, 16);
            System.BitConverter.GetBytes((short)1).CopyTo(header, 20);
            System.BitConverter.GetBytes((short)1).CopyTo(header, 22);
            System.BitConverter.GetBytes(16000).CopyTo(header, 24);
            System.BitConverter.GetBytes(16000 * 2).CopyTo(header, 28);
            System.BitConverter.GetBytes((short)2).CopyTo(header, 32);
            System.BitConverter.GetBytes((short)16).CopyTo(header, 34);
            System.Text.Encoding.ASCII.GetBytes("data").CopyTo(header, 36);
            System.BitConverter.GetBytes(bytesData.Length).CopyTo(header, 40);

            fileStream.Write(header, 0, header.Length);
            fileStream.Write(bytesData, 0, bytesData.Length);
        }

        Debug.Log("Saved WAV: " + filename);
    }
}