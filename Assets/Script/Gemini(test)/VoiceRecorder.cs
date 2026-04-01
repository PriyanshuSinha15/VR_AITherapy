using UnityEngine;

public class VoiceRecorder : MonoBehaviour
{
    public AudioClip recordedClip;
    private string micName;

    public void StartRecording()
    {
        micName = Microphone.devices[0];
        recordedClip = Microphone.Start(micName, false, 10, 16000);
        Debug.Log("Recording Started");
    }

    public void StopRecording()
    {
        Microphone.End(micName);
        Debug.Log("Recording Stopped");
    }
}   