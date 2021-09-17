using GameFrameWork.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Input
{
    public enum KeyType
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        A = 5,
        B = 6,
        X = 7,
        Y = 8,
        Start = 9,
        Select = 10,
        LB = 11,
        RB = 12,
        LT = 13,
        RT = 14,
        None = 15,
    }

    public enum AxisType
    {
        LeftAxis = 1,//左摇杆
        RightAxis = 2,//右摇杆
        CrossAxis = 3,//十字键
        None = 4,//
    }

    public class ComboKeyEventArgs : BaseEventArgs
    {
        public KeyType[] Keys { get; set; }
        public int EventId { get; set; }
        public GameFrameWorkAction<int, bool> KeyEvent { get; set; }

        public static ComboKeyEventArgs Create(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            ComboKeyEventArgs args = ReferencePool.Acquire<ComboKeyEventArgs>();
            args.Keys = keys;
            args.EventId = eventId;
            args.KeyEvent = keyEvent;
            return args;
        }

        public override void Clear()
        {
            Keys = null;
            KeyEvent = null;
        }
    }
}