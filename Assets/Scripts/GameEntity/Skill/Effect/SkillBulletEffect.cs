using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utility;

public class SkillBulletEffect : SkillBaseEffect
{
    public SkillBulletEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
    public override bool IsCompleted
    {
        get
        {
            return m_Owner.IsPlayComplete();
        }
    }

    public override void Effect(ISkillSelector selector)
    {
        for (int i = 0; i < m_SkillEffect.Bullets.Length; i++)
        {
            Bullet bullet = EntityMgr.Ins.GetEntity<Bullet>(m_SkillEffect.Bullets[i].Name);
            BulletData bulletData = ReferencePool.Acquire<BulletData>();
            bulletData.Health = 1;
            bulletData.MaxHealth = 1;
            bulletData.IsSmoon = m_SkillEffect.IsSmoon;
            bulletData.AddTargetForce = m_SkillEffect.AddTargetForce;
            bulletData.NormalAnim = m_SkillEffect.Bullets[i].NormalAnim;
            bulletData.HitAnim = m_SkillEffect.Bullets[i].HitAnim;
            bulletData.NormalAnimSpeed = m_SkillEffect.Bullets[i].NormalAnimSpeed;
            bulletData.HitAnimSpeed = m_SkillEffect.Bullets[i].HitAnimSpeed;
            bulletData.Dir = m_SkillEffect.Bullets[i].Dir;
            bulletData.Pos = m_SkillEffect.Bullets[i].Pos;
            bulletData.Velocity = m_SkillEffect.Bullets[i].Velocity;
            bulletData.HitRange = m_SkillEffect.Bullets[i].HitRange;
            bulletData.Drag = m_SkillEffect.Bullets[i].Drag;
            bulletData.IsPenatrate = m_SkillEffect.Bullets[i].IsPenatrate;
            bulletData.SkillExp = m_SkillData.EXP;
            bulletData.DamageMulity = m_SkillEffect.DamageMulity;
            bullet.SetData(bulletData);
            bullet.SetObjectType(ObjectType.CantBreakItem);
            bullet.SetOwner(m_Owner);
            bullet.SetRes(PathUtil.FormatPath(ResDefine.PREFAB_PATH, m_SkillEffect.Bullets[i].AssetName));
            bullet.SetLayer(LayerName.Unit);
        }
    }

    public override void Reset()
    {
    }

    public override void Exit()
    {
    }

    public override void Update(ISkillSelector selector)
    {

    }
}