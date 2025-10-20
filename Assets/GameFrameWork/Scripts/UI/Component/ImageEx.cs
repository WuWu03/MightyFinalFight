using System;
using GameFrameWork.Pool;
using GameFrameWork.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.UI
{
    [AddComponentMenu("UI/ImageEx")]
    public class ImageEx : Image
    {
        private string m_SpriteName;
        
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
                    SetSprite(value);
                }
                
                m_SpriteName = value;
            }
        }
        
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            PutSprite(m_SpriteName);
        }

        private void SetSprite(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.spriteName))
            {
                PutSprite(this.spriteName);
            }

            string spritePath = PathUtil.FormatPath(PathUtil.GetUISpritesPath(), spriteName);
            GameFrameWorkMgr.GetModule<IResourcePoolMgr>().Get<Sprite>(spritePath, OnSpriteLoaded);
        }

        private void PutSprite(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                return;
            }
            
            string spritePath = PathUtil.FormatPath(PathUtil.GetUISpritesPath(), spriteName);
            GameFrameWorkMgr.GetModule<IResourcePoolMgr>().Put(spritePath, sprite);
        }

        private void OnSpriteLoaded(string assetPath, UnityObject obj, object arg)
        {
            overrideSprite = obj as Sprite;
        }
    }
}