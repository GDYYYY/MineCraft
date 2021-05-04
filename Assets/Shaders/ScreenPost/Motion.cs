using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Motion : MonoBehaviour
{
    [Range(0.0f, 0.9f)]
    public float blurAmount = 0.5f;
	
    RenderTexture accumulationTexture;
    public Shader motionShader;
    
    [NonSerialized] private Material motion;

    void OnDisable() {
        DestroyImmediate(accumulationTexture);
    }
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (motion == null)
        {
            motion = new Material(Shader.Find("Unlit/MotionBlur"));
            motion.hideFlags = HideFlags.HideAndDontSave;
        }
        
        RenderTextureFormat format = src.format;
        if (accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height) {
            DestroyImmediate(accumulationTexture);
            accumulationTexture = new RenderTexture(src.width, src.height, 0,format);
            accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
            Graphics.Blit(src, accumulationTexture);
        }
        
        accumulationTexture.MarkRestoreExpected();

        motion.SetFloat("_BlurAmount", 1.0f - blurAmount);

        Graphics.Blit (src, accumulationTexture, motion);
        Graphics.Blit (accumulationTexture, dest);
        
    }
    
    public void changeState(Toggle toggle)
    {
        this.enabled = toggle.isOn;
    }
}
