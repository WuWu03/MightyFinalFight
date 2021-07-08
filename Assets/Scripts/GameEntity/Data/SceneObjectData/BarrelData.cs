public class BarrelData : SceneItemData
{
    public float MoveSpeed { get; set; }
    public int GroundY { get; set; }
    public float Dir { get; set; }
    public bool IsFloat { get; set; }
    public int ItemId { get; set; }

    public override void Clear()
    {
        base.Clear();
        MoveSpeed = 0;
        GroundY = 0;
        Dir = 0;
        IsFloat = false;
        ItemId = 0;
    }
}