using static SkillData;

public class SkillFactory
{
    public static SkillDeployer CreateDeployer(int skillID, BaseRole owner)
    {
        SkillDeployer ret = null;
        SkillData data = StaticConfig.SkillConfig.GetData(skillID);

        switch (data.Type)
        {
            case SkillType.NormalAttack:
                ret = new SkillNormalAttackDeployer(skillID, owner);
                break;
            case SkillType.JumpAttack:
                ret = new SkillJumpAttackDeployer(skillID, owner);
                break;
            case SkillType.SkillAttack:
                ret = new SkillSkillAttackDeployer(skillID, owner);
                break;
        }

        return ret;
    }
    public static ISkillSelector[] CreateSelector(SkillData.SkillEffect[] skillEffects)
    {
        ISkillSelector[] ret = new ISkillSelector[skillEffects.Length];
        for (int i = 0; i < skillEffects.Length; i++)
        {
            switch (skillEffects[i].SelectorType)
            {
                case SkillSelectorType.NearHitSelector:
                    ret[i] = new SkillNearHitSelector() { Index = i };
                    break;
                case SkillSelectorType.BulletSelector:
                    ret[i] = new SkillBulletSelector() { Index = i };
                    break;
            }
        }

        return ret;
    }

    public static ISkillEffect[] CreateEffects(SkillData.SkillEffect[] skillEffects)
    {
        ISkillEffect[] ret = new ISkillEffect[skillEffects.Length];

        for (int i = 0; i < skillEffects.Length; i++)
        {
            switch (skillEffects[i].EffectorType)
            {
                case SkillEffectorType.NearHitEffect:
                    ret[i] = new SkillNearHitEffect() { Index = i };
                    break;
                case SkillEffectorType.BulletHitEffect:
                    ret[i] = new SkillBulletEffect() { Index = i };
                    break;
                case SkillEffectorType.MoveHitEffect:
                    ret[i] = new SkillMoveHitEffect() { Index = i };
                    break;
                case SkillEffectorType.MoveTargetEffect:
                    ret[i] = new SkillMoveTargetEffect() { Index = i };
                    break;
            }
        }

        return ret;
    }

    public static bool CheckStatus(SkillStatus status, BaseRole owner)
    {
        bool ret = false;
        switch (status)
        {
            case SkillStatus.None:
                ret = true;
                break;
            case SkillStatus.Float:
                ret = owner.IsFloat;
                break;
            case SkillStatus.Ground:
                ret = owner.IsInGround;
                if(owner is BaseHero)
                    ret = ret && !(owner as BaseHero).IsCatch;
                break;
            case SkillStatus.Catch:
                ret = (owner as BaseHero).IsCatch;
                break;
        }

        return ret;
    }
}