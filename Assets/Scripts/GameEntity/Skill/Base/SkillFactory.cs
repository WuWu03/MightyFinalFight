using GameFrameWork;
using System.Text.RegularExpressions;
using UnityEngine;
using static SkillConfigData;

public class SkillFactory
{
    public static SkillBaseDeployer CreateDeployer(int skillId, BaseRole owner)
    {
        SkillBaseDeployer ret = null;
        SkillConfigData data = StaticConfig.SkillConfig.GetData(skillId);

        if (data == null)
        {
            Log.LogError("skill data is invalid skillId:", skillId);
            return null;
        }

        switch (data.DeployerType)
        {
            case SkillDeployerType.NormalAttack:
                ret = new SkillNormalAttackDeployer(skillId, owner);
                break;
            case SkillDeployerType.JumpAttack:
                ret = new SkillJumpAttackDeployer(skillId, owner);
                break;
            case SkillDeployerType.SkillAttack:
                ret = new SkillSkillAttackDeployer(skillId, owner);
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
                    isCondition = owner.isInGround;
                    break;
                case SkillPrevConditionType.DropGround:
                    isCondition = owner.isDropGround;
                    break;
                case SkillPrevConditionType.Float:
                    isCondition = owner.isFloat;
                    break;
                case SkillPrevConditionType.Catch:
                    isCondition = (owner as BaseHero).isCatch;
                    break;
                case SkillPrevConditionType.GroundNotCatch:
                    isCondition = owner.isInGround;
             
                    if (owner is BaseHero)
                        isCondition = isCondition && !(owner as BaseHero).isCatch;
                    else
                        isCondition = isCondition && !owner.isBeCatch;

                    break;
                case SkillPrevConditionType.HPMoreThan:
                    Match m1 = m_RegexHPMoreThan.Match(conditions[i].Args);
                    if (m1.Success) isCondition = owner.entityAttribute.health > int.Parse(m1.Groups[2].Value);
                    break;
                case SkillPrevConditionType.HPLessThan:
                    Match m2 = m_RegexHPLessThan.Match(conditions[i].Args);
                    if (m2.Success) isCondition = owner.entityAttribute.health < int.Parse(m2.Groups[2].Value);
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

    public static HurtData GetHurtData(ICanBeHit hit, BaseRole owner, SkillConfigData data, SkillEffect effect)
    {
        if (hit == null || !hit.canBeHit)
        {
            return null;
        }

        if (hit is BaseEnemy)//boss攻击优先级更高;
        {
            BaseEnemy baseEnemy = hit as BaseEnemy;
          
            if (baseEnemy.isBoss && baseEnemy.currCtrl.IsInSkill())
            {
                return null;
            }
        }

        float dir = (hit as BaseSceneObject).pos.x - owner.pos.x >= 0 ? 1 : -1;
        bool isCritical = false;
        bool isBoss = false;
        if (effect.ForceType == SkillAddForceType.SelfDir)
        {
            dir = owner.dir;
        }

        if(owner is BaseEnemy)
        {
            isBoss = (owner as BaseEnemy).isBoss;
        }

        HurtData hurtData = HurtData.Create();
        hurtData.id = data.Id;
        hurtData.skillExp = data.EXP;
        hurtData.attackerDir = owner.dir;
        hurtData.attackForce = new Vector2(effect.AddTargetForce.x * dir, effect.AddTargetForce.y);
        hurtData.attackerPos = owner.pos;
        hurtData.canBeDefense = effect.CanBeDefense;
        hurtData.isSwoon = effect.IsSmoon;
        hurtData.attackerId = owner.id;
        hurtData.attackValue = CacDamage(owner.entityAttribute.attackValue, hit.entityAttribute.defenseValue, owner.entityAttribute.criticalValue, effect.DamageMulity, out isCritical);
        hurtData.isCritical = isCritical;
        hurtData.hurtSound = data.HurtSound;
        hurtData.hurtAnim = string.Empty;
        hurtData.isGroundHurt = effect.IsOnGroundHurt;
        hurtData.isBoss = isBoss;

        return hurtData;
    }

    public static bool SkillHit(ICanBeHit hit,BaseRole owner, SkillConfigData data, SkillEffect effect)
    {
        HurtData hurtData = GetHurtData(hit, owner, data, effect);

        if (hurtData != null)
        {
            hit.OnHurtMsg(hurtData);
            return !hit.IsHurtWillDie(hurtData.attackValue);
        }

        return false;
    }

    public static int CacDamage(int attack, int defense, int critical, float mulity, out bool isCritical)
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

        string str = "[基础伤害: " + baseDamage + "]" +
                     "\n[最终伤害： " + damage + "]" +
                     "\n[攻击: " + attack + "]" +
                     "\n[防御: " + defense + "]" +
                     "\n[暴击率: " + critical + "]" +
                     "\n[倍率: " + (mulity > 0 ? mulity : 1) + "]" +
                     "\n[偏移: " + fluctuate + "]" +
                     "\n[是否暴击： " + isCri + "]";

        Log.LogInfo(str);
        isCritical = isCri;
        return Mathf.FloorToInt(damage);
    }

    public static Vector2 GetSmoonForce(float dir = 1f)
    {
        return new Vector2(40f * dir, 150f);
    }

    private static Regex m_RegexHPMoreThan = new Regex(@"(HPMoreThan:)([0-9]+)");
    private static Regex m_RegexHPLessThan = new Regex(@"(HPLessThan:)([0-9]+)");
}