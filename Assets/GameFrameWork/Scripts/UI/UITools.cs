using GameFrameWork.Pool;
using GameFrameWork.Resources;
using GameFrameWork.Utility;
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
            return PathUtil.FormatPath(ResDefine.UI_PATH, name);
        }

        public static void LoadSprite(string name, Image renderer)
        {
            string realPath = PathUtil.FormatPath(ResDefine.ICON_PATH, name);
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
