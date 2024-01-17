using GameFrameWork;
using System.Text.RegularExpressions;
using UnityEngine;
using static SkillConfigData;

public static class SkillUtil
{
    public static bool IsRectangleCollide(Rect rect1, Rect rect2)
    {
        return IsRectangleCollide(rect1.center.x, rect1.center.y, rect1.width, rect1.height, rect2.center.x, rect2.center.y, rect2.width, rect2.height);
    }

    public static bool IsRectangleCollide(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
    {
        bool xCheck = Mathf.Abs(x1 - x2) <= (w1 + w2) / 2;
        bool yCheck = Mathf.Abs(y1 - y2) <= (h1 + h2) / 2;
        return xCheck && yCheck;
    }

    public static bool CheckStatus(SkillPrevCondition[] conditions, BaseRole owner)
    {
        bool ret = true;
        if (conditions == null || conditions.Length < 1)
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

            if (isRevert)
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

        if (hit is BaseEnemy)//boss¹¥»÷ÓÅÏÈ¼¶¸ü¸ß;
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

        if (owner is BaseEnemy)
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

    public static bool SkillHit(ICanBeHit hit, BaseRole owner, SkillConfigData data, SkillEffect effect)
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

        string str = "[»ù´¡ÉËº¦: " + baseDamage + "]" +
                     "\n[×îÖÕÉËº¦£º " + damage + "]" +
                     "\n[¹¥»÷: " + attack + "]" +
                     "\n[·ÀÓù: " + defense + "]" +
                     "\n[±©»÷ÂÊ: " + critical + "]" +
                     "\n[±¶ÂÊ: " + (mulity > 0 ? mulity : 1) + "]" +
                     "\n[Æ«ÒÆ: " + fluctuate + "]" +
                     "\n[ÊÇ·ñ±©»÷£º " + isCri + "]";

        Log.LogInfo(Color.red, str);
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
