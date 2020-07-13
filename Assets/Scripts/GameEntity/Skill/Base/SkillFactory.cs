using System.Text.RegularExpressions;
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
                case SkillEffectorType.SubHP:
                    ret[i] = new SkillSubHPEffect() { Index = i };
                    break;
            }
        }

        return ret;
    }

    public static bool CheckStatus(SkillPrevCondition[] conditions, BaseRole owner)
    {
        bool ret = true;
        if(conditions == null || conditions.Length < 1)
        {
            return ret;
        }

        for (int i = 0; i < conditions.Length; i++)
        {
            bool isCondition = false;

            switch (conditions[i].Status)
            {
                case SkillStatus.None:
                    isCondition = true;
                    break;
                case SkillStatus.Float:
                    isCondition = owner.IsFloat;
                    break;
                case SkillStatus.Ground:
                    isCondition = owner.IsInGround;
                    if (owner is BaseHero)
                        isCondition = isCondition && !(owner as BaseHero).IsCatch;
                    break;
                case SkillStatus.Catch:
                    isCondition = (owner as BaseHero).IsCatch;
                    break;
                case SkillStatus.HPMoreThan:
                    Match m = m_RegexHPMoreThan.Match(conditions[i].Args);               
                    if (m.Success)
                    {
                        string[] str = m.Value.Split(':');
                        isCondition = owner.Health > int.Parse(str[1]);
                    }
                    break;
            }

            if (!isCondition)
            {
                ret = false;
                break;
            }
        }

        return ret;
    }

    private static Regex m_RegexHPMoreThan = new Regex(@"HPMoreThan:[0-9]+");
    private static Regex m_RegexHPLessThan = new Regex(@"HPLessThan:[0-9]+");
}