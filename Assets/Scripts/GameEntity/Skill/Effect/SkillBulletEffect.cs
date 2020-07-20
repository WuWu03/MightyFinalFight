using FrameWork.Pool;
public class SkillBulletEffect : SkillBaseEffect
{
    public SkillBulletEffect(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
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
            Bullet bullet = SceneObjectPool.Ins.Get<Bullet>(m_SkillEffect.Bullets[i].Name);        
            bullet.InitData(new BulletData()
            {
                Health = 1,
                MaxHealth = 1,
                IsSmoon = m_SkillEffect.IsSmoon,
                AddTargetForce = m_SkillEffect.AddTargetForce,
                NormalAnim = m_SkillEffect.Bullets[i].NormalAnim,
                HitAnim = m_SkillEffect.Bullets[i].HitAnim,
                NormalAnimSpeed = m_SkillEffect.Bullets[i].NormalAnimSpeed,
                HitAnimSpeed = m_SkillEffect.Bullets[i].HitAnimSpeed,
                Dir = m_SkillEffect.Bullets[i].Dir,
                Pos = m_SkillEffect.Bullets[i].Pos,
                Velocity = m_SkillEffect.Bullets[i].Velocity,
                HitRange = m_SkillEffect.Bullets[i].HitRange,
                Drag = m_SkillEffect.Bullets[i].Drag,
                TriggerOffest = m_SkillEffect.Bullets[i].TriggerOffest,
                TriggerSize = m_SkillEffect.Bullets[i].TriggerSize,
            });
            bullet.SetObjectType(ObjectType.CantBreakItem);
            bullet.SetOwner(m_Owner);
            bullet.SetRes(string.Format("{0}/{1}", ResDefine.EFFECT_PATH, m_SkillEffect.Bullets[i].Name));
        }
    }

    public override void Reset()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {

    }
}