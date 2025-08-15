using GameFrameWork;

public class BaseEnemyData : BaseRoleData
{
    public string[] hurtAnims { get; set; }
    public int hpBarWdith { get; set; }
    public bool isBoss { get; set; }

    public new static BaseEnemyData Create()
    {
        return ReferencePool.Acquire<BaseEnemyData>();
    }

    public override void Clear()
    {
        base.Clear();
        hurtAnims = null;
        hpBarWdith = 0;
        isBoss = false;
    }
}
