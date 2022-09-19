using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleMask : MonoBehaviour
{
    public RectTransform clipRect;
    //public Material mt;
    public Material[] mt;
    private void Awake()
    {
        ParticleSystem[] system = GetComponentsInChildren<ParticleSystem>();
        mt = new Material[system.Length];
        for (int i = 0; i < system.Length; i++)
        {
            mt[i] = system[i].GetComponent<Renderer>().material;
        }
        //mask = GetComponentInParent<Mask>();
        // ScrollView位置变化时重新计算裁剪区域
        //GetComponentInParent<ScrollRect>().onValueChanged.AddListener((e) => { setClip(); });
    }

    private void OnEnable()
    {
        setClip();
    }
    private void Start()
    {
        setClip();
    }

    void setClip()
    {
        for (int i = 0; i < mt.Length; i++)
        {
            if (!mt[i].shader.name.Contains("Particle_Additive_Clip")) continue;
            Vector3[] wc = new Vector3[4];
            this.clipRect.GetWorldCorners(wc);        // 计算world space中的点坐标
            var clipRect = new Vector4(wc[0].x, wc[0].y, wc[2].x, wc[2].y);// 选取左下角和右上角
            mt[i].SetVector("_ClipRect", clipRect);                           // 设置裁剪区域
            mt[i].SetFloat("_UseClipRect", 1.0f); // 开启裁剪
        }
    }
}