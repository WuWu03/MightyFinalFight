using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotToRenderTexture : MonoBehaviour
{
    public int textureWidth = 128;
    public int textureHeight = 256;
    public int blurSize = 1;
    public int blurCount = 1;
    public int showCount = 0;

    public Material blurMaterial;
    public RenderTexture outPutRenderTexture = null;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture buffer0 = RenderTexture.GetTemporary(textureWidth, textureHeight, 0);
        buffer0.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, buffer0);

        for (int i = 0; i < blurCount; i++)
        {
            blurMaterial.SetFloat("_BlurSize", (float)blurSize + (float)i * blurSize);
            RenderTexture buffer1 = RenderTexture.GetTemporary(textureWidth, textureHeight, 0);
            Graphics.Blit(buffer0, buffer1, blurMaterial, 0);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            if(blurMaterial.passCount > 1)
            {
                buffer1 = RenderTexture.GetTemporary(textureWidth, textureHeight, 0);
                Graphics.Blit(buffer0, buffer1, blurMaterial, 1);
                RenderTexture.ReleaseTemporary(buffer0);
                buffer0 = buffer1;
            }
        }

        Graphics.Blit(buffer0, outPutRenderTexture);
        RenderTexture.ReleaseTemporary(buffer0);
        Graphics.Blit(source, destination);
    }
}
