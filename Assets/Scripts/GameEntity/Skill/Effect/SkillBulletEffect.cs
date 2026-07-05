using WuWuFramework.Utils;

public class SkillBulletEffect : SkillBaseEffect
{
    public SkillBulletEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
        for (int i = 0; i < m_SkillEffect.Bullets.Length; i++)
        {
            Bullet bullet = GameEntry.entityMgr.GetEntity<Bullet>(m_SkillEffect.Bullets[i].Name);
            BulletData bulletData = BulletData.Create();
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
            bullet.SetLayer(LayerName.Unit);
            bullet.SetOwner(m_Owner);
            bullet.SetAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, m_SkillEffect.Bullets[i].AssetName));
        }

        Complete();
    }
    
    public bool BulletEffect(ICanBeHit hit)
    {
        return SkillUtil.SkillHit(hit, m_Owner, m_SkillData, m_SkillEffect);
    }
}