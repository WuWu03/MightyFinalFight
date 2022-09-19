using GameFrameWork;

public abstract class BaseSceneObjectData : BaseEventArgs
{
    public int entityId { get; set; }

    public override void Clear()
    {
        entityId = 0;
    }
}