using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Event
{
    public class GameEventArgs : BaseEventArgs
    {
        public override void Clear()
        {

        }

        public override BaseEventArgs Clone()
        {
            return null;
        }
    }
}