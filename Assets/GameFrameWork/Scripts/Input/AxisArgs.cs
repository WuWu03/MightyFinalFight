using GameFrameWork.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class AxisArgs : BaseEventArgs
    {
        public string Horizontal { get; set; }
        public string Vertical { get; set; }

        public static AxisArgs Create(string horizontal, string vertical)
        {
            AxisArgs args = ReferencePool.Acquire<AxisArgs>();
            args.Horizontal = horizontal;
            args.Vertical = vertical;
            return args;
        }

        public override void Clear()
        {
            Horizontal = null;
            Vertical = null;
        }
    }
}