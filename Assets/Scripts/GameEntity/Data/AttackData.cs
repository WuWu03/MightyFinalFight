using FrameWork;
using System;

public class AttackData : BaseEventArgs
{
    public string AnimationName { get; set; }
    public float Dir { get; set; }
    public bool CanChangeDir { get; set; }
    public float AnimSpeed { get; set; }
    public override BaseEventArgs Clone()
    {
        return Activator.CreateInstance<AttackData>();
    }
}