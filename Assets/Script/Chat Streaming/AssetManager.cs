using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance;

    public bool isFinalScene;
    public GameObject chatText;
    public GameObject myText;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isFinalScene)
        {
            chatText.SetActive(false);
            myText.SetActive(false);
        }
        else
        {
            chatText.SetActive(true);
            myText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
