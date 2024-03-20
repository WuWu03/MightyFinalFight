using GameFrameWork;

public class BaseRoleData : BaseSceneObjectData
{
    public bool isCatchControl { get; set; }

    public static BaseRoleData Create()
    {
        return ReferencePool.Acquire<BaseRoleData>();
    }

    public override void Clear()
    {
        base.Clear();
        isCatchControl = false;
    }
}
