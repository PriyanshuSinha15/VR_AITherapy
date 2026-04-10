using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SSEDownloadHandler : DownloadHandlerScript
{
    public Action<string> OnChunk;
    public Action OnComplete;
    public Action<string> OnError;

    private StringBuilder buffer = new StringBuilder();

    public SSEDownloadHandler() : base(new byte[1024]) { }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        if (data == null || dataLength == 0)
            return false;

        string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
        buffer.Append(chunk);

        string content = buffer.ToString();

        // Split SSE events
        string[] events = content.Split(new string[] { "\n\n" }, StringSplitOptions.None);

        // Keep last incomplete chunk
        buffer.Clear();
        buffer.Append(events[events.Length - 1]);

        for (int i = 0; i < events.Length - 1; i++)
        {
            string evt = events[i];

            if (!evt.Contains("data:")) continue;

            string line = evt.Trim();

            string payload = line.Replace("data:", "").Trim();

            // ✅ DONE
            if (payload == "[DONE]")
            {
                OnComplete?.Invoke();
                return true;
            }

            // ✅ JSON string chunk ("Hello")
            if (payload.StartsWith("\""))
            {
                string text = JsonConvert.DeserializeObject<string>(payload);
                OnChunk?.Invoke(text);
                continue;
            }

            // ✅ Error object
            if (payload.StartsWith("{"))
            {
                var obj = JObject.Parse(payload);

                if (obj["error"] != null)
                {
                    OnError?.Invoke(obj["error"].ToString());
                }
            }
        }

        return true;
    }


}

