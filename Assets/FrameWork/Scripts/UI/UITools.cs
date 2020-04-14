using FrameWork.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public static class UITools
    {
        public static void SetIconSprite(string path, Image renderer)
        {
            Sprite sprite = null;

            if (m_DicSprite.TryGetValue(path, out sprite))
            {
                renderer.sprite = sprite;
                return;
            }

            Action<UnityEngine.Object> action = delegate (UnityEngine.Object obj)
            {
                renderer.sprite = obj as Sprite;

                if (!m_DicSprite.ContainsKey(path))
                {
                    m_DicSprite.Add(path, renderer.sprite);
                }
            };

            string loadPath = string.Format("{0}/{1}", ResDefine.ICON_PATH, path);
            
            ResMgr.Ins.LoadAsset(loadPath, action, true, typeof(Sprite));
        }

        private static Dictionary<string, Sprite> m_DicSprite = new Dictionary<string, Sprite>();
    }
}
