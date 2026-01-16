using Avaturn.Core.Runtime.Scripts.Avatar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRMultiMeshLipSync : MonoBehaviour
{
    private const string HEAD_MESH = "Head_Mesh";
    private const string TONGUE_MESH = "Tongue_Mesh";
    private const string TEETH_MESH = "Teeth_Mesh";

    [Header("Required Scripts")]
    public OVRLipSyncContext ovrLipSyncContext;
    public PrepareAvatar prepareAvatar;

    [Header("Player Transform")]
    public Transform playerTransform;

    [Header("Meshes")]
    public SkinnedMeshRenderer head;
    public SkinnedMeshRenderer tongue;
    public SkinnedMeshRenderer teeth;

    [Header("Mappings")]
    public int[] headMap = new int[15];
    public int[] tongueMap = new int[15];
    public int[] teethMap = new int[15];

    private float[] weights = new float[15];

    [Header("Smooth speed for blending")]
    public float smoothing = 12;

    private void Start()
    {
        prepareAvatar.OnModelPrepared += PrepareAvatar_OnModelPrepared;
    }

    private void PrepareAvatar_OnModelPrepared(object sender, System.EventArgs e)
    {
        TrySetTargetMeshes();
    }

    private void LateUpdate()
    {
        if (ovrLipSyncContext == null) return;
        var frame = ovrLipSyncContext.GetCurrentPhonemeFrame();
        if (frame == null) return;
        if (head == null || tongue == null || teeth == null) return;

        for (int i = 0; i < 15; i++)
        {
            float target = frame.Visemes[i];

            weights[i] = Mathf.Lerp(weights[i], target, smoothing * Time.deltaTime);

            ApplyBlendShapeWeight(head, headMap[i], weights[i]);
            ApplyBlendShapeWeight(teeth, teethMap[i], weights[i]);
            ApplyBlendShapeWeight(tongue, tongueMap[i], weights[i]);
        }
    }

    void ApplyBlendShapeWeight(SkinnedMeshRenderer mesh, int index, float weight)
    {
        if(mesh == null || index < 0) return;
        mesh.SetBlendShapeWeight(index, weight);
    }

    private void TrySetTargetMeshes()
    {
        var SMRenders = playerTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach(SkinnedMeshRenderer smr in SMRenders)
        {
            //Debug.Log(smr.name);
            if (smr.name == HEAD_MESH)
                head = smr;
            else if(smr.name == TEETH_MESH)
                teeth = smr;
            else if(smr.name == TONGUE_MESH)
                tongue = smr;
        }
    }
}
