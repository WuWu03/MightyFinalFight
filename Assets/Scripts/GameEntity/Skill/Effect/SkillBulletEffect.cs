using FrameWork.Pool;
using Runtime.Config;

namespace Runtime
{
    public class SkillBulletEffect : ISkillEffect
    {
        public bool IsCompleted
        {
            get
            {
                if(m_Owner != null && m_Owner.ActorAnimator.animation.isCompleted)
                {
                    m_Owner = null;
                    return true;
                }
                return false;
            }
        }

        public void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector)
        {
            
            for (int i = 0; i < skillData.Bullets.Length; i++)
            {
                Bullet bullet = ObjectPool.Ins.Get<Bullet>(skillData.Bullets[i].Name);
                bullet.SetBulletInfo(owner, skillData, skillData.Bullets[i]);
            }

            m_Owner = owner;
        }

        private BaseRole m_Owner = null;
    }
}
