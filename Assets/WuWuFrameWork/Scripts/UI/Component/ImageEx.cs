using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using WuWuFramework.Pool;
using WuWuFramework.Utils;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.UI
{
    [AddComponentMenu("UI/ImageEx")]
    public class ImageEx : Image
    {
        [SerializeField] private string m_SpriteName;
        private string m_CurrAtlasPath;
        private SpriteAtlas m_CurrSpriteAtlas;

        public string spriteName
        {
            get
            {
                return m_SpriteName;
            }
            set
            {
                if (m_SpriteName != value)
                {
                    m_SpriteName = value;
                    PutAtlas();
                    LoadAtlas(m_SpriteName);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying && WuWuFrameworkEntry.isStartUp && !string.IsNullOrEmpty(m_SpriteName) && m_CurrSpriteAtlas == null)
            {
                LoadAtlas(m_SpriteName);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PutAtlas();
        }

        private void LoadAtlas(string spriteName)
        {
            m_CurrAtlasPath = PathUtil.FormatPath(PathUtil.GetUIAtlasPath(), Path.GetDirectoryName(spriteName), ".spriteatlasv2");
            WuWuFrameworkMgr.GetModule<IResourcePoolMgr>().Get<SpriteAtlas>(m_CurrAtlasPath, OnSpriteAtlasLoaded);
        }

        private void PutAtlas()
        {
            if (m_CurrSpriteAtlas != null)
            {
                WuWuFrameworkMgr.GetModule<IResourcePoolMgr>().Put(m_CurrAtlasPath, m_CurrSpriteAtlas);
                m_CurrSpriteAtlas = null;
                m_CurrAtlasPath = null;
            }
        }

        private void OnSpriteAtlasLoaded(string assetPath, UnityObject obj, object arg)
        {
            m_CurrSpriteAtlas = obj as SpriteAtlas;

            if (m_CurrAtlasPath != assetPath)
            {
                PutAtlas();
                return;
            }

            overrideSprite = m_CurrSpriteAtlas.GetSprite(Path.GetFileNameWithoutExtension(m_SpriteName));
        }
    }
}