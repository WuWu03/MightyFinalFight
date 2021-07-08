using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotToRenderTexture : MonoBehaviour
{
    public int TextureWidth = 128;
    public int TextureHeight = 256;
    public Material BlurMaterial;
    public int BlurSize = 1;
    public int BlurCount = 1;
    public RenderTexture OutPutRenderTexture = null;
    public int ShowCount = 0;



    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture buffer0 = RenderTexture.GetTemporary(TextureWidth, TextureHeight, 0);
        buffer0.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, buffer0);

        for (int i = 0; i < BlurCount; i++)
        {
            BlurMaterial.SetFloat("_BlurSize", (float)BlurSize + (float)i * BlurSize);
            RenderTexture buffer1 = RenderTexture.GetTemporary(TextureWidth, TextureHeight, 0);
            Graphics.Blit(buffer0, buffer1, BlurMaterial, 0);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            if(BlurMaterial.passCount > 1)
            {
                buffer1 = RenderTexture.GetTemporary(TextureWidth, TextureHeight, 0);
                Graphics.Blit(buffer0, buffer1, BlurMaterial, 1);
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }
        }

        Graphics.Blit(buffer0, OutPutRenderTexture);
        RenderTexture.ReleaseTemporary(buffer0);
        Graphics.Blit(source, destination);
    }
}
