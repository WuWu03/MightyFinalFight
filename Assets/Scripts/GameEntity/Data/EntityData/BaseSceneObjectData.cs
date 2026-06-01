using WuWuFramework;

public abstract class BaseSceneObjectData : WuWuFrameworkEventArg
{
    public int entityId { get; set; }

    public override void Clear()
    {
        entityId = 0;
    }
}