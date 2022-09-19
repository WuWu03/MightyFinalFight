using UnityEngine;

namespace GameFrameWork.Resources
{
    public class SpritePool : ResourcesPool<Sprite, SpritePool>
    {
        protected override bool m_NeedInstantiate { get { return false; } }
    }
}