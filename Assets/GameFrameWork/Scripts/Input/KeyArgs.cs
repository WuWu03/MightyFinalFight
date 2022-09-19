using GameFrameWork.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class KeyArgs : BaseEventArgs
    {
        public string keyName { get; set; }
        public bool isShift { get; set; }
        public KeyType replaceKeyType { get; set; }

        public static KeyArgs Create(string keyName, KeyType replaceKeyType, bool isShift)
        {
            KeyArgs args = ReferencePool.Acquire<KeyArgs>();
            args.keyName = keyName;
            args.replaceKeyType = replaceKeyType;
            args.isShift = isShift;
            return args;
        }

        public override void Clear()
        {
            keyName = null;
            isShift = false;
        }
    }
}