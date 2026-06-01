using UnityEngine;

namespace WuWuFramework.UI
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleMask : MonoBehaviour
    {
        public RectTransform clipRect;
        public Material[] materials;
        private void Awake()
        {
            ParticleSystem[] system = GetComponentsInChildren<ParticleSystem>(true);
            materials = new Material[system.Length];

            for (int i = 0; i < system.Length; i++)
            {
                materials[i] = system[i].GetComponent<Renderer>().material;
            }
            //mask = GetComponentInParent<Mask>();
            // ScrollView位置变化时重新计算裁剪区域
            //GetComponentInParent<ScrollRect>().onValueChanged.AddListener((e) => { setClip(); });
        }

        private void OnEnable()
        {
            SetClip();
        }
        private void Start()
        {
            SetClip();
        }

        private void SetClip()
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (!materials[i].shader.name.Contains("Particle_Additive_Clip"))
                {
                    continue;
                }

                Vector3[] wc = new Vector3[4];
                this.clipRect.GetWorldCorners(wc);        // 计算world space中的点坐标
                var clipRect = new Vector4(wc[0].x, wc[0].y, wc[2].x, wc[2].y);// 选取左下角和右上角
                materials[i].SetVector("_ClipRect", clipRect);                           // 设置裁剪区域
                materials[i].SetFloat("_UseClipRect", 1.0f); // 开启裁剪
            }
        }
    }
}