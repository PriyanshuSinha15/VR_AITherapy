using Avaturn.Core.Runtime.Scripts.Avatar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateHumanoidAvatar : MonoBehaviour
{
    public Animator playerAnim;
    void Start()
    {
        if(playerAnim.avatar == null)
            playerAnim.avatar = GetAvatar();
    }

    public UnityEngine.Avatar GetAvatar() =>
        HumanoidAvatarBuilder.Build(gameObject);
}
