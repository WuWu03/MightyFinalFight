using GameFrameWork;

public abstract class BaseSceneObjectData : BaseEventArgs
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    public override void Clear()
    {
        Health = 0;
        MaxHealth = 0;
    }
}