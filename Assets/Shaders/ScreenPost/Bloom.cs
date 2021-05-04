using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Bloom : MonoBehaviour
{
    [Range(0, 10)]
    public float intensity = 1;
    [Range(0, 10)]
    public float threshold = 1;
    [Range(1, 16)]
    public int iterations = 4;
    [Range(0, 1)]
    public float softThreshold = 0.5f;
    public Shader bloomShader;
    RenderTexture[] textures = new RenderTexture[16];
    [NonSerialized] private Material bloom;
     void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (bloom == null)
        {
            bloom = new Material(Shader.Find("Unlit/Bloom"));
            bloom.hideFlags = HideFlags.HideAndDontSave;
        }
        
        float k=threshold * softThreshold;
        Vector4 filter;
        filter.x = threshold;
        filter.y = filter.x - k;
        filter.z = 2f * k;
        filter.w = 0.25f / (k + 0.00001f);
        bloom.SetVector("_Filter",filter);
        bloom.SetFloat("_Intensity",Mathf.GammaToLinearSpace(intensity));

        int width = src.width / 2;
        int height = src.height / 2;
        RenderTextureFormat format = src.format;

        RenderTexture curDst = textures[0] = RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(src,curDst,bloom,0);
        RenderTexture curSrc = curDst;

        int i;
        for (i = 1; i < iterations; i++)
        {
            width /= 2;
            height /= 2;
            if (height < 2) break;
            curDst=textures[i]= RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(curSrc,curDst,bloom,1);
            curSrc = curDst;
        }

        for (i -= 2; i >= 0; i--)
        {
            curDst = textures[i];
            textures[i] = null;
            Graphics.Blit(curSrc,curDst,bloom,2);
            RenderTexture.ReleaseTemporary(curSrc);
            curSrc = curDst;
        }
        
        bloom.SetTexture("_SourceTex",src);
        Graphics.Blit(curSrc,dest,bloom,3);
        
        RenderTexture.ReleaseTemporary(curSrc);
    }

     public void changeState(Toggle toggle)
     {
         this.enabled = toggle.isOn;
     }
}
