using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class VertexGeminiTest : MonoBehaviour
{
    [TextArea]
    public string accessToken;   // paste token here

    public TMP_InputField inputField;

    string url =
        "https://us-central1-aiplatform.googleapis.com/v1/projects/project-23eb238f-b5dd-4390-9d5/locations/us-central1/publishers/google/models/gemini-2.0-flash:generateContent";

    public void AskGemini(string message)
    {
        StartCoroutine(CallGemini(message));
    }

    public void GenerateResponseFromInputFieldText()
    {
        AskGemini(inputField.text);
    }

    IEnumerator CallGemini(string message)
    {
        string body =
            "{\"contents\":[{\"role\":\"user\",\"parts\":[{\"text\":\"" +
            message + "\"}]}]}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");

        byte[] raw = Encoding.UTF8.GetBytes(body);

        req.uploadHandler = new UploadHandlerRaw(raw);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Authorization", "Bearer " + accessToken);
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Gemini Response: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError(req.error);
            Debug.LogError(req.downloadHandler.text);
        }
    }
}