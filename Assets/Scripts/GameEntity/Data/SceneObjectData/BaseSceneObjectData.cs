using GameFrameWork;

public abstract class BaseSceneObjectData : BaseEventArgs
{
    public int EntityId { get; set; }

    public override void Clear()
    {
        EntityId = 0;
    }
}