using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 屏幕截图
/// </summary>
public class Screenshot : MonoBehaviour
{
    public int downSample = 2;
    public RawImage rawImage;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void Shot()
    {
        StartCoroutine(ReadScreenPixels());
    }

    public void ClearShot()
    {
        RenderTexture.ReleaseTemporary(m_RenderTexture);
    }

    private IEnumerator ReadScreenPixels()
    {
        yield return new WaitForEndOfFrame();

        int width = (int)(m_RectTransform.rect.width);
        int height = (int)(m_RectTransform.rect.height);
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply(false, true);
        RenderImage(texture2D);
    }

    private void RenderImage(Texture2D texture2D)
    {
        int rtW = texture2D.width / downSample;
        int rtH = texture2D.height / downSample;

        if (m_RenderTexture == null)
        {
            m_RenderTexture = RenderTexture.GetTemporary(rtW, rtH, 0);
            m_RenderTexture.filterMode = FilterMode.Bilinear;
        }

        Graphics.Blit(texture2D, m_RenderTexture);

        if (rawImage != null)
        {
            rawImage.texture = m_RenderTexture;
        }
    }

    private RenderTexture m_RenderTexture = null;
    private RectTransform m_RectTransform = null;
}