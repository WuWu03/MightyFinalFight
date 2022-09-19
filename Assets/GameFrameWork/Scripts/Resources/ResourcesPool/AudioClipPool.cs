using UnityEngine;

namespace GameFrameWork.Resources
{
    public class AudioClipPool : ResourcesPool<AudioClip, AudioClipPool>
    {
        protected override bool m_NeedInstantiate { get { return false; } }
    }
}