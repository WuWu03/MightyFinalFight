using FrameWork.Pool;
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
            SpritePool.Ins.Get(path, (Sprite sprite) =>
            {
                renderer.sprite = sprite;
            });
        }
    }
}
