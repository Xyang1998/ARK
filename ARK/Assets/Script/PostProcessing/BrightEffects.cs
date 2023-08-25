using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightEffects : PostEffectsBase
{
    public Shader shader;
    private Material material;

    public Material Material
    {
        get
        {
            material = CheckShaderAndCreateMaterial(shader, material);
            return material;
        }
    }

    public float brightness = 1.0f;
    public float saturation = 1.0f;
    public float contrast = 1.0f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Material != null)
        {
            Material.SetFloat("_Brightness",brightness);
            Material.SetFloat("_Saturation",saturation);
            Material.SetFloat("_Contrast",contrast);
            Graphics.Blit(src,dest,Material);
        }
        else{
            Graphics.Blit(src,dest);
        } 
    }
}
