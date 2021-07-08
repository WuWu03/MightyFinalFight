using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class SpritePool : ResPool<Sprite, SpritePool>
    {
        protected override bool m_NeedInstantiate { get { return false; } }
    }
}