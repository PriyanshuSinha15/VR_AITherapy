using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ClearInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInputField; 

    public void ClearChatInputField()
    {
        chatInputField.text = string.Empty; 
    }
}
