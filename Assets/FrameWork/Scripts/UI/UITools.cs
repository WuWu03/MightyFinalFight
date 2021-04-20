using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameWork.UI
{
    public static class UITools
    {
        public static string GetUIResPath(string name)
        {
            return string.Format("{0}/{1}", ResDefine.UI_PATH, name);
        }

        public static void LoadSprite(string path,string name, Image renderer)
        {
            string realPath = string.Format("{0}/{1}/{2}", ResDefine.ICON_PATH, path, name);
            SpritePool.Ins.Get(realPath, (Sprite sprite,object[] param) =>
            {
                renderer.sprite = sprite;
            });
        }

        public static void LoadUI(string uiName,Action<GameObject,object[]> loadCallback,params object[] param)
        {
            GameObjectPool.Ins.Get(GetUIResPath(uiName), loadCallback, param);
        }
    }
}
