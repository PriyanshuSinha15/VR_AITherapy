using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wave : MonoBehaviour
{
    private const string WAVE_TRIGGER = "Wave";

    [SerializeField] private Button waveButton;

    private Animator anim;
    void Start()
    {
        waveButton.onClick.AddListener(() =>
        {
            WaveAnim();
        });

        anim = GetComponent<Animator>();
    }

    public void WaveAnim()
    {
        anim.SetTrigger(WAVE_TRIGGER);
    }
}
