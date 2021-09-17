using GameFrameWork.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class KeyArgs : BaseEventArgs
    {
        public string KeyName { get; set; }
        public bool IsShift { get; set; }
        public KeyType ReplaceKeyType { get; set; }

        public static KeyArgs Create(string keyName, KeyType replaceKeyType, bool isShift)
        {
            KeyArgs args = ReferencePool.Acquire<KeyArgs>();
            args.KeyName = keyName;
            args.ReplaceKeyType = replaceKeyType;
            args.IsShift = isShift;
            return args;
        }

        public override void Clear()
        {
            KeyName = null;
            IsShift = false;
        }
    }
}