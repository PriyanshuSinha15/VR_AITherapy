using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRMultiMeshLipSync : MonoBehaviour
{
    public OVRLipSyncContext ovrLipSyncContext;

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

    private void LateUpdate()
    {
        if (ovrLipSyncContext == null) return;
        var frame = ovrLipSyncContext.GetCurrentPhonemeFrame();
        if (frame == null) return;
        if (head == null || tongue == null || teeth == null) return;

        for (int i = 0; i < 15; i++)
        {
            float target = frame.Visemes[i] * 100;

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
}
