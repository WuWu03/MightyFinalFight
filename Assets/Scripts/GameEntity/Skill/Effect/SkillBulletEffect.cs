using FrameWork.Pool;
public class SkillBulletEffect : ISkillEffect
{
    public bool IsCompleted
    {
        get
        {
            if (m_Owner != null && m_Owner.IsPlayComplete())
            {
                m_Owner = null;
                return true;
            }
            return false;
        }
    }

    public int Index
    {
        get;
        set;
    }

    public void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector)
    {
        for (int i = 0; i < skillData.SkillEffects[Index].Bullets.Length; i++)
        {
            Bullet bullet = SceneObjectPool.Ins.Get<Bullet>(skillData.SkillEffects[Index].Bullets[i].Name);
            bullet.SetObjectType(ObjectType.SceneItem);
            bullet.SetBulletInfo(owner, skillData.SkillEffects[Index], skillData.SkillEffects[Index].Bullets[i]);
        }

        m_Owner = owner;
    }

    public void Reset()
    {


    }

    public void Exit()
    {
        
    }

    private BaseRole m_Owner = null;
}