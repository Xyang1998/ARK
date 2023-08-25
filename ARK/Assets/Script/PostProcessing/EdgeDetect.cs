using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetect : PostEffectsBase
{
    public float edgeOnly = 1.0f;
    public Color edgeColor=Color.black;
    public Color backGroundColor=Color.white;
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
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Material != null)
        {
            Material.SetFloat("_EdgeOnly",edgeOnly);
            Material.SetColor("_EdgeColor",edgeColor);
            Material.SetColor("_BackGroundColor",backGroundColor);
            Graphics.Blit(src,dest,Material);
        }
        else{
            Graphics.Blit(src,dest);
        } 
    }

}
