using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    public RawImage rawImage;
    public Camera cam;

    private RenderTexture m_renderTexture;

    private void Start()
    {
        m_renderTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);

        rawImage.texture = m_renderTexture;
        cam.targetTexture = m_renderTexture;
        //必须为 CameraClearFlags.SolidColor或CameraClearFlags.Depth，CameraClearFlags.Nothing 时会不显示
        cam.clearFlags = CameraClearFlags.SolidColor;

        //CameraClearFlags.SolidColor时会有背景色，需要设置背景色透明
        Color color = cam.backgroundColor;
        color.a = 0f;
        cam.backgroundColor = color;
    }
}