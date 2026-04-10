using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WhisperSTT : MonoBehaviour
{
    [Header("OpenAI")]
    public string apiKey = "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";

    [Header("Audio File Path")]
    public string filePath = "C:\\Users\\priya\\Desktop\\WhisperTesting/Priyansh.wav";  

    public void StartTranscription()
    {
        StartCoroutine(SendAudio());
    }

    IEnumerator SendAudio()
    {
        byte[] audioData = System.IO.File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "audio.wav");
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
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}