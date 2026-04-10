using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;
    public string Token;
    public string SessionID;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        Token = PlayerPrefs.GetString("token", "");
        SessionID = PlayerPrefs.GetString("sessionId", "");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
