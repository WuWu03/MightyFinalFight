using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameFrameWork.Event
{
    public class GameEventArgs : BaseEventArgs
    {
        public static GameEventArgs Create(int id)
        {
            GameEventArgs args = new GameEventArgs();
            args.id = id;
            return args;
        }

        public override void Clear()
        {
            id = 0;
        }
    }
}