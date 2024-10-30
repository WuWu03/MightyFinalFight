using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对_rawImage所在区域截屏并模糊处理
/// </summary>
public class ScreenshotToRenderTexture : MonoBehaviour
{
    public RawImage _rawImage;

    public void OnButtonClick()
    {
        RectTransform rectTransform = this.transform.GetComponent<RectTransform>();
        BlurRegion(rectTransform);
    }

    /// <summary>
    /// uiRoot是界面根节点
    /// </summary>
    private void BlurRegion(RectTransform uiRoot, int iterations = 3, float blurSpread = 2.0f, int downSample = 2)
    {
        //先将_rawImage的透明度改为0，不影响截屏
        Color c = _rawImage.color;
        c.a = 0;
        _rawImage.color = c;
        StartCoroutine(BlurRegionCoroutine(uiRoot, _rawImage, iterations, blurSpread, downSample));
    }

    /// <summary>
    /// 注意：根节点这里Canvas的Render mode为Overlay，其他模式，Canvas的宽高和屏幕宽高如果不一致，则需要转换
    /// </summary>
    private IEnumerator BlurRegionCoroutine(RectTransform uiRoot, RawImage rawImage, int iterations, float blurSpread, int downSample)
    {
        yield return new WaitForEndOfFrame();

        RectTransform imageRt = rawImage.rectTransform;
        int imageWidth = (int)(imageRt.rect.width);
        int imageHeight = (int)(imageRt.rect.height);
        Texture2D texture2D = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);

        //计算rawImage作为uiRoot子物体时的局部坐标，因为rawImage可能是孙子节点，所以需要转换
        Vector3 imagePos = uiRoot.InverseTransformPoint(rawImage.transform.position);
        //计算rawImage左下角坐标，屏幕左下角为原点（0, 0）
        Vector2 imagePivot = imageRt.pivot;
        float leftBottomX = Screen.width * 0.5f - imageWidth * imagePivot.x + imagePos.x;
        float leftBottomY = Screen.height * 0.5f - imageHeight * imagePivot.y + imagePos.y;

        //从屏幕读取像素, leftBottomX，leftBottomY 是读取的初始位置，width，height是读取像素的宽度和高度
        texture2D.ReadPixels(new Rect(leftBottomX, leftBottomY, imageWidth, imageHeight), 0, 0);
        texture2D.Apply(false, true);

        //使用《Shader入门精要》中用到的高斯模糊
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/GameFrameWork/Shaders/GaussianBlur.shader");
        if (shader == null || rawImage == null)
            yield break;
        var material = new Material(shader);
        if (material == null)
            yield break;

        RenderImage(rawImage, iterations, blurSpread, downSample, material, texture2D);
        Color c = rawImage.color;
        c.a = 1;
        rawImage.color = c;
    }

    private void RenderImage(RawImage rawImage, int iterations, float blurSpread, int downSample, Material material, Texture2D texture2D)
    {
        int rtW = texture2D.width / downSample;
        int rtH = texture2D.height / downSample;

        // 首先定义了第一个缓存buffer0，并把src中的图像缩放后存储到buffer0中。在迭代过程中，我们又定义了第二个缓存buffer1。
        // 在执行第一个Pass时，输入是buffer0，输出是buffer1，完毕后首先把buffer0释放，再把结果值buffer1存储到buffer0中，
        // 重新分配buffer1，然后再调用第二个Pass，重复上述过程。迭代完成后，buffer0将存储最终的图像
        RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;
        Graphics.Blit(texture2D, buffer0);

        for (int i = 0; i < iterations; i++)
        {
            material.SetFloat("_BlurSize", 1.0f + i * blurSpread);
            RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

            // Render the vertical pass
            Graphics.Blit(buffer0, buffer1, material, 0);

            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;
            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

            // Render the horizontal pass
            Graphics.Blit(buffer0, buffer1, material, 1);

            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;
        }

        var buffer = RenderTexture.GetTemporary(buffer0.descriptor); //最终效果
        Graphics.Blit(buffer0, buffer);
        RenderTexture.ReleaseTemporary(buffer0);
        rawImage.texture = buffer;
    }
}