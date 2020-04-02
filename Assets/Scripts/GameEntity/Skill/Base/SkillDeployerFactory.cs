using static SkillData;

namespace Runtime
{
    public class SkillDeployerFactory
    {
        public static ISkillSelector CreateSelector(SkillSelectorType type)
        {
            ISkillSelector ret = null;
            switch (type)
            {
                case SkillSelectorType.NearHitSelector:
                    ret =  new SkillNearHitSelector();
                    break;
                case SkillSelectorType.BulletSelector:
                    ret = new SkillBulletSelector();
                    break;
            }

            return ret;
        }

        public static ISkillEffect[] CreateEffects(SkillEffectorType[] types)
        {
            ISkillEffect[] ret = new ISkillEffect[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                switch (types[i])
                {
                    case SkillEffectorType.NearHitEffect:
                        ret[i] = new SkillNearHitEffect();
                        break;
                    case SkillEffectorType.BulletHitEffect:
                        ret[i] = new SkillBulletEffect();
                        break;
                    case SkillEffectorType.MoveHitEffect:
                        ret[i] = new SkillMoveHitEffect();
                        break;
                }
            }

            return ret;
        }
    }
}
