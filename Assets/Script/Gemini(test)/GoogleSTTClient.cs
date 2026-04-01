using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GoogleSTTClient : MonoBehaviour
{
    public string accessToken;
    public VoiceRecorder recorder;

    public void SendToSTT()
    {
        StartCoroutine(ProcessSTT());
    }

    IEnumerator ProcessSTT()
    {
        byte[] wav = WavUtility.FromAudioClip(recorder.recordedClip);
        string base64 = Convert.ToBase64String(wav);

        string json =
        @"{
          ""config"": {
            ""encoding"": ""LINEAR16"",
            ""sampleRateHertz"": 16000,
            ""languageCode"": ""en-US""
          },
          ""audio"": {
            ""content"": """ + base64 + @"""
          }
        }";

        UnityWebRequest req =
            new UnityWebRequest(
                "https://speech.googleapis.com/v1/speech:recognize",
                "POST");

        byte[] body = Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Authorization", "Bearer " + accessToken);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("STT Response:\n" + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError(req.error);
            Debug.LogError(req.downloadHandler.text);
        }
    }
}