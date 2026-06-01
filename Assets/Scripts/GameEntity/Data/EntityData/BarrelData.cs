using WuWuFramework;

public class BarrelData : SceneItemData
{
    public float moveSpeed { get; set; }
    public int groundY { get; set; }
    public float dir { get; set; }
    public bool isFloat { get; set; }
    public int itemId { get; set; }

    public new static BarrelData Create()
    {
        return ReferencePool.Acquire<BarrelData>();
    }

    public override void Clear()
    {
        base.Clear();
        moveSpeed = 0;
        groundY = 0;
        dir = 0;
        isFloat = false;
        itemId = 0;
    }
}