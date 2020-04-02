using FrameWork.Pool;


namespace Runtime
{
    public class SkillBulletEffect : ISkillEffect
    {
        public bool IsCompleted
        {
            get
            {
                if(m_Avatar.ActorAnimator.animation.isCompleted)
                {
                    m_Avatar = null;
                    return true;
                }
                return false;
            }
        }

        public void Effect(BaseAvatar owner, SkillData skillData, ISkillSelector selector)
        {
            for (int i = 0; i < skillData.Bullets.Length; i++)
            {
                Bullet bullet = ObjectPool.Ins.Get<Bullet>(skillData.Bullets[i].Name);
                bullet.SetBulletInfo(owner, skillData, skillData.Bullets[i]);
            }

            m_Avatar = owner;
        }

        private BaseAvatar m_Avatar = null;
    }
}
