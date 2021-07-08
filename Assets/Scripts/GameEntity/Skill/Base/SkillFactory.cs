using System.Text.RegularExpressions;
using UnityEngine;
using static SkillConfigData;

public class SkillFactory
{
    public static SkillBaseDeployer CreateDeployer(int skillID, BaseRole owner)
    {
        SkillBaseDeployer ret = null;
        SkillConfigData data = StaticConfig.SkillConfig.GetData(skillID);

        switch (data.DeployerType)
        {
            case SkillDeployerType.NormalAttack:
                ret = new SkillNormalAttackDeployer(skillID, owner);
                break;
            case SkillDeployerType.JumpAttack:
                ret = new SkillJumpAttackDeployer(skillID, owner);
                break;
            case SkillDeployerType.SkillAttack:
                ret = new SkillSkillAttackDeployer(skillID, owner);
                break;
        }

        return ret;
    }

    public static ISkillSelector[] CreateSelector(SkillConfigData skillData, BaseRole owner)
    {
        ISkillSelector[] ret = new ISkillSelector[skillData.SkillEffects.Length];
        for (int i = 0; i < skillData.SkillEffects.Length; i++)
        {
            switch (skillData.SkillEffects[i].SelectorType)
            {
                case SkillSelectorType.None:
                    ret[i] = new SkillNoneSelector(skillData, owner, i);
                    break;
                case SkillSelectorType.NearHitSelector:
                    ret[i] = new SkillNearHitSelector(skillData, owner, i);
                    break;
                case SkillSelectorType.BulletSelector:
                    ret[i] = new SkillBulletSelector(skillData, owner, i);
                    break;
            }
        }

        return ret;
    }

    public static ISkillEffect[] CreateEffects(SkillConfigData skillData, BaseRole owner)
    {
        ISkillEffect[] ret = new ISkillEffect[skillData.SkillEffects.Length];

        for (int i = 0; i < skillData.SkillEffects.Length; i++)
        {
            switch (skillData.SkillEffects[i].EffectorType)
            {
                case SkillEffectorType.None:
                    ret[i] = new SkillNoneEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.NearHitEffect:
                    ret[i] = new SkillNearHitEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.BulletHitEffect:
                    ret[i] = new SkillBulletEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.MoveHitEffect:
                    ret[i] = new SkillMoveHitEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.MoveTargetEffect:
                    ret[i] = new SkillMoveTargetEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.MoveSelfEffect:
                    ret[i] = new SkillNoneEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.SubHP:
                    ret[i] = new SkillSubHPEffect(skillData, owner, i);
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
            SkillPrevConditionType status = conditions[i].PrevConditionType;
            switch (status)
            {
                case SkillPrevConditionType.None:
                    isCondition = true;
                    break;
                case SkillPrevConditionType.Ground:
                    isCondition = owner.IsInGround;
                    if (owner is BaseHero)
                        isCondition = isCondition && !(owner as BaseHero).IsCatch;
                    else if (owner is BaseEnemy)
                        isCondition = isCondition && !(owner as BaseEnemy).IsBeCatch;
                    break;
                case SkillPrevConditionType.DropGround:
                    Debug.Log(owner.IsDropGround);
                    isCondition = owner.IsDropGround;
                    break;
                case SkillPrevConditionType.Float:
                    isCondition = owner.IsFloat;
                    break;
                case SkillPrevConditionType.Catch:
                    isCondition = (owner as BaseHero).IsCatch;
                    break;
                case SkillPrevConditionType.GroundOrCatch:
                    isCondition = owner.IsInGround || (owner as BaseHero).IsCatch;
                    break;
                case SkillPrevConditionType.HPMoreThan:
                    Match m1 = m_RegexHPMoreThan.Match(conditions[i].Args);
                    if (m1.Success) isCondition = owner.Health > int.Parse(m1.Groups[2].Value);
                    break;
                case SkillPrevConditionType.HPLessThan:
                    Match m2 = m_RegexHPLessThan.Match(conditions[i].Args);
                    if (m2.Success) isCondition = owner.Health < int.Parse(m2.Groups[2].Value);
                    break;
                default:
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

    private static Regex m_RegexHPMoreThan = new Regex(@"(HPMoreThan:)([0-9]+)");
    private static Regex m_RegexHPLessThan = new Regex(@"(HPLessThan:)([0-9]+)");
}