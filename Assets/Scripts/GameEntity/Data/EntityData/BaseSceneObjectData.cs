using GameFrameWork;

public abstract class BaseSceneObjectData : GameFrameWorkEventArg
{
    public int entityId { get; set; }

    public override void Clear()
    {
        entityId = 0;
    }
}