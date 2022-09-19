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
        public KeyType[] keys { get; set; }
        public int eventId { get; set; }
        public GameFrameWorkAction<int, bool> keyEvent { get; set; }

        public static ComboKeyEventArgs Create(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            ComboKeyEventArgs args = ReferencePool.Acquire<ComboKeyEventArgs>();
            args.keys = keys;
            args.eventId = eventId;
            args.keyEvent = keyEvent;
            return args;
        }

        public override void Clear()
        {
            keys = null;
            keyEvent = null;
        }
    }
}