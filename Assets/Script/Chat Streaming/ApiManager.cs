using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json.Linq;
using TMPro;

public class ApiManager : MonoBehaviour
{
    [Header("Scene Difference")]
    public bool isFinalScene;

    [Header("Chat Streaming")]
    public TMP_Text chatText;
    public TMP_InputField chatInputField;
    public string message;

    [Header("Login")]
    string baseUrl = "https://api.mindsai.live";
    public string token = "";
    string email = "user@example.com";
    string password = "password123";

    [Header("Chat Session")]
    public string sessionId = "";

    //[Header("Open AI")]
    private string apiKey = "";

    public void Start()
    {
        if(PlayerPrefs.HasKey("token"))
        {
            Debug.Log("Already Logged In");
        }
        else
        {
            Login();
        }

    }



    #region Login
    public void Login()
    {
        StartCoroutine(LoginCoroutine(email, password));
    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        string url = baseUrl + "/api/auth/login";

        LoginRequest body = new LoginRequest
        {
            identifier = email,
            password = password,
            loginType = "password"
        };

        string jsonBody = JsonUtility.ToJson(body);

        Debug.Log("Sending JSON: " + jsonBody); // 👈 MUST CHECK

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;

            Debug.Log("Login Response: " + response);

            var json = JObject.Parse(response);

            token = json["data"]["token"].ToString();

            Debug.Log("TOKEN: " + token);

            //Set Token Value in AuthManager
            AuthManager.Instance.Token = token;

            // Save token
            PlayerPrefs.SetString("token", token);
            PlayerPrefs.Save();

            CreateSession();
        }
        else
        {
            Debug.LogError("Error: " + request.downloadHandler.text);
        }
    }
    #endregion Login

    #region Chat Session
    public void CreateSession()
    {
        StartCoroutine(CreateSessionCoroutine());
    }

    IEnumerator CreateSessionCoroutine()
    {
        string url = baseUrl + "/api/chat/sessions";
        string token = AuthManager.Instance.Token;

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request Failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
            yield break;
        }

        string response = request.downloadHandler.text;
        Debug.Log("RAW RESPONSE: " + response);

        var json = JObject.Parse(response);

        if (json["data"] == null || json["data"]["session"] == null || json["data"]["session"]["_id"] == null)
        {
            Debug.LogError("Invalid response structure!");
            yield break;
        }

        sessionId = json["data"]["session"]["_id"].ToString();

        Debug.Log("SESSION ID: " + sessionId);

        // Set Session ID in AuthManager
        AuthManager.Instance.SessionID = sessionId;

        //Save Session ID 
        PlayerPrefs.SetString("sessionId", sessionId);
        PlayerPrefs.Save();
    }

    #endregion Chat Session

    #region Chat Message

    //public void SendChat()
    //{
    //    StartStreaming(chatInputField.text);
    //}
    public void StartStreaming()
    {
        StartCoroutine(StreamCoroutine(message));
    }

    IEnumerator StreamCoroutine(string message)
    {
        string url = baseUrl + "/api/vrchat/stream";

        string token = AuthManager.Instance.Token;
        string sessionId = AuthManager.Instance.SessionID;

        // ✅ Build body
        var body = new
        {
            message = message,
            sessionId = sessionId,
            platform = "unity"
        };

        string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

        Debug.Log("STREAM BODY: " + jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new SSEDownloadHandler();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Accept", "text/event-stream");

        SSEDownloadHandler handler = (SSEDownloadHandler)request.downloadHandler;

        string fullResponse = "";

        handler.OnChunk += (chunk) =>
        {
            Debug.Log("AI Chunk: " + chunk);

            fullResponse += chunk;

            // 👉 Update UI here
            OnAIResponseUpdated(fullResponse);
        };

        handler.OnComplete += () =>
        {
            Debug.Log("✅ STREAM COMPLETE");

            OnAIResponseComplete(fullResponse);
        };

        handler.OnError += (err) =>
        {
            Debug.LogError("❌ STREAM ERROR: " + err);
        };

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request Failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    void OnAIResponseUpdated(string text)
    {
        if (isFinalScene)
        {

        }
        else
        {
            chatText.text = text;
        }
    }

    void OnAIResponseComplete(string finalText)
    {
        Debug.Log("Final AI Response: " + finalText);
        //Connecting to Generate speech

        string continousText = finalText.Replace("\n", " ");
        StartCoroutine(GenerateSpeech(continousText));
    }
    #endregion  Chat Message

    #region TextToSpeech
    IEnumerator GenerateSpeech(string text)
    {
        string url = "https://api.openai.com/v1/audio/speech";

        //apiKey = "sk-xxxxxxxx"; // ⚠️ TEMP ONLY (move to backend later)

        var body = new
        {
            model = "gpt-4o-mini-tts",
            voice = "alloy",
            input = text
        };

        string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS Error: " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            byte[] audioData = request.downloadHandler.data;

            StartCoroutine(PlayAudio(audioData));
        }
    }

    IEnumerator PlayAudio(byte[] audioData)
    {
        string path = Application.persistentDataPath + "/ai_speech.mp3";

        System.IO.File.WriteAllBytes(path, audioData);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio Load Error: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                AudioSource source = GetComponent<AudioSource>();
                source.clip = clip;
                source.Play();
            }
        }
    }

    #endregion TextToSpeech

    [System.Serializable]
    public class LoginRequest
    {
        public string identifier;
        public string password;
        public string loginType;
    }
}