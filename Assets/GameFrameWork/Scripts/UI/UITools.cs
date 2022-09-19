using GameFrameWork.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public static class UITools
    {
        public static void InitUIResPath(UIResPath uiResPath)
        {
            m_uiResPath = uiResPath;
        }

        public static string GetUIResPath(string name)
        {
            return m_uiResPath.GetUIResPath(name);
        }

        public static void SetSprite(this Image renderer, string name)
        {
            string realPath = m_uiResPath.GetUISpritePath(name);
            SpritePool.instance.Get(realPath, (Sprite sprite, object[] param) =>
            {
                renderer.sprite = sprite;
            });
        }

        public static void LoadUI(string uiName, GameFrameWorkAction<GameObject, object[]> loadCallback, params object[] param)
        {
            GameObjectPool.instance.Get(GetUIResPath(uiName), loadCallback, param);
        }

        private static UIResPath m_uiResPath = null;
    }

    public class UIResPath
    {
        public virtual string GetUIResPath(string name)
        {
            return string.Empty;
        }

        public virtual string GetUISpritePath(string name)
        {
            return string.Empty;
        }
    }
}
