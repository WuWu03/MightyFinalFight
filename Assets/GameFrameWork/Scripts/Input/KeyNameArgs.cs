using GameFrameWork.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public class KeyNameArgs : BaseEventArgs
    {
        public string KeyName { get; set; }
        public bool IsShift { get; set; }
        public KeyType ReplaceKeyType { get; set; }

        public static KeyNameArgs Create(string keyName, KeyType replaceKeyType, bool isShift)
        {
            KeyNameArgs args = ReferencePool.Acquire<KeyNameArgs>();
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