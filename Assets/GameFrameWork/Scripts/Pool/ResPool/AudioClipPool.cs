using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Pool
{
    public class AudioClipPool : ResPool<AudioClip, AudioClipPool>
    {
        protected override bool m_NeedInstantiate { get { return false; } }
    }
}