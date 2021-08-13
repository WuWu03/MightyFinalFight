using GameFrameWork.Log;
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
                case SkillEffectorType.TargetPositionEffect:
                    ret[i] = new SkillTargetTransformEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.TargetScaleEffect:
                    ret[i] = new SkillTargetTransformEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.SelfTransformEffect:
                    ret[i] = new SkillSelfTransformEffect(skillData, owner, i);
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
            bool isRevert = conditions[i].IsRevert;
            switch (status)
            {
                case SkillPrevConditionType.None:
                    isCondition = true;
                    break;
                case SkillPrevConditionType.Ground:
                    isCondition = owner.IsInGround;
                    break;
                case SkillPrevConditionType.DropGround:
                    isCondition = owner.IsDropGround;
                    break;
                case SkillPrevConditionType.Float:
                    isCondition = owner.IsFloat;
                    break;
                case SkillPrevConditionType.Catch:
                    isCondition = (owner as BaseHero).IsCatch;
                    break;
                case SkillPrevConditionType.GroundNotCatch:
                    isCondition = owner.IsInGround;
             
                    if (owner is BaseHero)
                        isCondition = isCondition && !(owner as BaseHero).IsCatch;
                    else
                        isCondition = isCondition && !owner.IsBeCatch;

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

            if(isRevert)
            {
                isCondition = !isCondition;
            }

            if (!isCondition)
            {
                ret = false;
                break;
            }
        }

        return ret;
    }

    public static int CacDamage(int attack, int defense, int critical, float mulity)
    {
        int a = 2;
        int b = 1;
        float criMulity = 1.5f;
        float fluctuate = Random.Range(0.8f, 1.1f);
        bool isCri = Random.Range(1, 101) <= critical;
        float baseDamage = Mathf.Max(a * attack - b * defense, 0);
        float damage = baseDamage * fluctuate * (mulity > 0 ? mulity : 1);

        if (isCri)
        {
            damage *= criMulity;
        }

        string str =   "[基础伤害: " + baseDamage + "]" +
                     "\n[最终伤害： " + damage + "]" +
                     "\n[攻击: " + attack + "]" +
                     "\n[防御: " + defense + "]" +
                     "\n[暴击率: " + critical + "]" +
                     "\n[倍率: " + (mulity > 0 ? mulity : 1) + "]" +
                     "\n[偏移: " + fluctuate + "]" +
                     "\n[是否暴击： " + isCri + "]";

        GameFrameworkLog.Log(str);
        return Mathf.FloorToInt(damage);
    }

    private static Regex m_RegexHPMoreThan = new Regex(@"(HPMoreThan:)([0-9]+)");
    private static Regex m_RegexHPLessThan = new Regex(@"(HPLessThan:)([0-9]+)");
}