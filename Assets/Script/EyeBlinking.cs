using Avaturn.Core.Runtime.Scripts.Avatar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeBlinking : MonoBehaviour
{
    private const string EYE_MESH = "Eye_Mesh";
    private const string EYEAO_MESH = "EyeAO_Mesh";
    private const string EYELASH_MESH = "Eyelash_Mesh";
    private const string HEAD_MESH = "Head_Mesh";

    public PrepareAvatar prepareAvatar;

    [SerializeField] private SkinnedMeshRenderer eyeMesh;
    [SerializeField] private SkinnedMeshRenderer eyeAOMesh;
    [SerializeField] private SkinnedMeshRenderer eyeLashMesh;
    [SerializeField] private SkinnedMeshRenderer headMesh;

    [SerializeField] private int eyeMeshCloseIndex = 8;
    [SerializeField] private int eyeAOMeshCloseIndex = 29;
    [SerializeField] private int eyeLashMeshCloseIndex = 23;
    [SerializeField] private int headMeshCloseIndex = 20;

    [Header("Target Weight for Blendshape")]
    [SerializeField] private float blendShapeWeight;

    [Header("Speed of Blinking")]
    [SerializeField] private float blinkingSpeed = 6;
    [SerializeField] private float blinkingResetSpeed = 8;

    [Header("Timer")]
    [SerializeField] private float maxTimer = 5;
    [SerializeField] private float maxBlinkDuration = 2;
    float currentTimer;
    float blinkDuration;

    [Header("Testing")]
    [SerializeField] private bool setupDone = false;


    private void Start()
    {
        prepareAvatar.OnModelPrepared += PrepareAvatar_OnModelPrepared;

        currentTimer = maxTimer;
        blinkDuration = maxBlinkDuration;

        TrySetTargetMeshes();
    }

    private void PrepareAvatar_OnModelPrepared(object sender, System.EventArgs e)
    {
        TrySetTargetMeshes();
    }

    private void Update()
    {
        if (setupDone)
        {
            Blinking();
        }
    }

    private void Blinking()
    {
        if (eyeMesh == null || eyeAOMesh == null || eyeLashMesh == null || headMesh == null) return;

        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0)
        {
            if (blinkDuration > 0)
            {
                //Blink Eye 
                blinkDuration -= Time.deltaTime;
                SetBlendShapeForEyeBlinking();
            }
            else
            {
                //Stop Blinking Eye
                blinkDuration = maxBlinkDuration;
                currentTimer = maxTimer;
            }
        }
        else
        {
            // Reset eye blend shape weight
            ResetBlendShape();
        }
    }

    private void SetBlendShapeForEyeBlinking()
    {
        blendShapeWeight = Mathf.PingPong(blinkingSpeed * Time.time, 1);
        ApplyBlendShapeWeight(blendShapeWeight);
    }

    private void ResetBlendShape()
    {
        blendShapeWeight = Mathf.Lerp(blendShapeWeight, 0, blinkingResetSpeed * Time.deltaTime);
        ApplyBlendShapeWeight(blendShapeWeight);
    }

    private void ApplyBlendShapeWeight(float blendShapeWeight)
    {
        eyeMesh.SetBlendShapeWeight(eyeMeshCloseIndex, blendShapeWeight);
        eyeAOMesh.SetBlendShapeWeight(eyeAOMeshCloseIndex, blendShapeWeight);
        eyeLashMesh.SetBlendShapeWeight(eyeLashMeshCloseIndex, blendShapeWeight);
        headMesh.SetBlendShapeWeight(headMeshCloseIndex, blendShapeWeight);
    }

    public void TrySetTargetMeshes()
    {
        foreach (SkinnedMeshRenderer s in transform.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (s.transform.name == EYE_MESH)
            {
                eyeMesh = s;
            }
            else if (s.transform.name == EYEAO_MESH)
            {
                eyeAOMesh = s;
            }
            else if (s.transform.name == EYELASH_MESH)
            {
                eyeLashMesh = s;
            }
            else if (s.transform.name == HEAD_MESH)
            {
                headMesh = s;
            }
        }

        if (eyeMesh != null && eyeAOMesh != null && eyeLashMesh != null && headMesh != null)
        {
            Debug.Log("Setup For Eyeblinking Done");
            //DebugBlendShapeExistOrNot(eyeMesh);
            setupDone = true;
        }
    }

    void DebugBlendShapeExistOrNot(SkinnedMeshRenderer smr)
    {
        var mesh = smr.sharedMesh;
        Debug.Log($"BlendShapeCount : {mesh.blendShapeCount}");

        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            Debug.Log(mesh.GetBlendShapeName(i));
        }
    }

}
