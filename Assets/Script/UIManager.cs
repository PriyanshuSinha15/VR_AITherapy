using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startRecording;
    [SerializeField] private Button stopRecording;
    // Start is called before the first frame update
    void Start()
    {
        startRecording.gameObject.SetActive(true);
        stopRecording.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRecording()
    {
        startRecording.gameObject.SetActive(false);
        stopRecording.gameObject.SetActive(true);
    }

    public void StopRecording()
    {
        startRecording.gameObject.SetActive(true);
        stopRecording.gameObject.SetActive(false);
    }
}
