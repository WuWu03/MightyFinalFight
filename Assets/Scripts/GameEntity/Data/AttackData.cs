using FrameWork;
using System;

namespace Runtime
{
    public class AttackData: BaseEventArgs
    {
        public string AnimationName { get; set; }
        public float Dir { get; set; }
        public bool CanChangeDir { get; set; }

        public override BaseEventArgs Clone()
        {
            return Activator.CreateInstance<AttackData>();
        }
    }
}
