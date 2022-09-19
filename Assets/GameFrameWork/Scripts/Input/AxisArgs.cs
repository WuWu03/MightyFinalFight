using GameFrameWork.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class AxisArgs : BaseEventArgs
    {
        public string horizontal { get; set; }
        public string vertical { get; set; }

        public static AxisArgs Create(string horizontal, string vertical)
        {
            AxisArgs args = ReferencePool.Acquire<AxisArgs>();
            args.horizontal = horizontal;
            args.vertical = vertical;
            return args;
        }

        public override void Clear()
        {
            horizontal = null;
            vertical = null;
        }
    }
}