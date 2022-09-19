using GameFrameWork;
using GameFrameWork.GameEntity;
using GameFrameWork.Utilities;

public class SkillBulletEffect : SkillBaseEffect
{
    public SkillBulletEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
        for (int i = 0; i < m_SkillEffect.Bullets.Length; i++)
        {
            Bullet bullet = EntityMgr.instance.GetEntity<Bullet>(m_SkillEffect.Bullets[i].Name);
            BulletData bulletData = ReferencePool.Acquire<BulletData>();
            bulletData.bulletIndex = i;
            bulletData.normalAnim = m_SkillEffect.Bullets[i].NormalAnim;
            bulletData.hitAnim = m_SkillEffect.Bullets[i].HitAnim;
            bulletData.normalAnimSpeed = m_SkillEffect.Bullets[i].NormalAnimSpeed;
            bulletData.hitAnimSpeed = m_SkillEffect.Bullets[i].HitAnimSpeed;
            bulletData.dir = m_SkillEffect.Bullets[i].Dir;
            bulletData.pos = m_SkillEffect.Bullets[i].Pos;
            bulletData.velocity = m_SkillEffect.Bullets[i].Velocity;
            bulletData.hitRange = m_SkillEffect.Bullets[i].HitRange;
            bulletData.drag = m_SkillEffect.Bullets[i].Drag;
            bulletData.isPenatrate = m_SkillEffect.Bullets[i].IsPenatrate;

            bullet.SetSkillEffect(this);
            bullet.SetData(bulletData);
            bullet.SetObjectType(ObjectType.CantBreakItem);
            bullet.SetOwner(m_Owner);
            bullet.SetRes(PathUtil.FormatPath(ResDefine.PrefabPath, m_SkillEffect.Bullets[i].AssetName));
            bullet.SetLayer(LayerName.Unit);
        }

        Complete();
    }

    public void BulletEffect(ICanBeHit hit)
    {
        SkillFactory.SkillHit(hit, m_Owner, m_SkillData, m_SkillEffect);
    }
}