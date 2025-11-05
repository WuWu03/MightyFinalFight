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

    public static bool IsPointInTriangle1(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 pv1 = point - v1;
        Vector3 pv2 = point - v2;
        Vector3 pv3 = point - v3;

        Vector3 v21 = v2 - v1;
        Vector3 v32 = v3 - v2;
        Vector3 v13 = v1 - v3;

        var npab = Vector3.Cross(pv1, v21).y;
        var npbc = Vector3.Cross(pv2, v32).y;
        var npca = Vector3.Cross(pv3, v13).y;

        return (npab * npbc) > 0 && (npab * npca) > 0;
    }

    public static bool IsPointInTriangle2(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float area = Vector3.Cross(v2 - v1, v3 - v1).magnitude / 2f;
        float area1 = Vector3.Cross(v2 - point, v3 - point).magnitude / 2f;
        float area2 = Vector3.Cross(v3 - point, v1 - point).magnitude / 2f;
        float area3 = Vector3.Cross(v1 - point, v2 - point).magnitude / 2f;
        float sum = area1 + area2 + area3;
        return Mathf.Approximately(sum, area);
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
                    isCondition = (owner as BaseHero).isCatching;
                    break;
                case SkillPrevConditionType.GroundNotCatch:
                    isCondition = owner.isInGround;

                    if (owner is BaseHero)
                        isCondition = isCondition && !(owner as BaseHero).isCatching;
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

    public static HurtStateArg GetHurtData(ICanBeHit hit, BaseRole owner, SkillConfigData data, SkillEffect effect,bool isPause)
    {
        if (hit == null || !hit.canBeHit)
        {
            return null;
        }

        if (hit is BaseEnemy baseEnemy)//boss攻击优先级更高;
        {
            if (baseEnemy.isBoss && baseEnemy.skillMgr.IsInSkill())
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

        HurtStateArg hurtArg = HurtStateArg.Create();
        hurtArg.skillExp = data.EXP;
        hurtArg.attackerDir = owner.dir;
        hurtArg.attackForce = new Vector2(effect.AddTargetForce.x * dir, effect.AddTargetForce.y);
        hurtArg.attackerPos = owner.pos;
        hurtArg.canBeDefense = effect.CanBeDefense;
        hurtArg.isSwoon = effect.IsSmoon;
        hurtArg.attackerId = owner.entityID;
        hurtArg.attackValue = CacDamage(owner.entityAttribute.attackValue, hit.entityAttribute.defenseValue, owner.entityAttribute.criticalValue, effect.DamageMulity, out isCritical);
        hurtArg.isCritical = isCritical;
        hurtArg.hurtSound = data.HurtSound;
        hurtArg.hurtAnim = string.Empty;
        hurtArg.isGroundHurt = effect.IsOnGroundHurt;
        hurtArg.isPause = isPause;
        hurtArg.isBoss = isBoss;

        return hurtArg;
    }

    public static bool SkillHit(ICanBeHit hit, BaseRole owner, SkillConfigData data, SkillEffect effect , bool isPause = false)
    {
        HurtStateArg hurtArg = GetHurtData(hit, owner, data, effect, isPause);
        bool result = false;

        if (hurtArg != null)
        {
            hit.HurtState(hurtArg);
            result = !hit.IsHurtWillDie(hurtArg.attackValue);
        }

        return result;
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
                     "  [最终伤害： " + damage + "]" +
                     "  [攻击: " + attack + "]" +
                     "  [防御: " + defense + "]" +
                     "  [暴击率: " + critical + "]" +
                     "  [倍率: " + (mulity > 0 ? mulity : 1) + "]" +
                     "  [偏移: " + fluctuate + "]" +
                     "  [是否暴击： " + isCri + "]";

        Log.LogInfo(Color.red, str);
        isCritical = isCri;
        return Mathf.FloorToInt(damage);
    }

    public static Vector2 GetSmoonForce(float dir = 1f)
    {
        return new Vector2(40f * dir, 150f);
    }

    public static Vector2 GetFloatSmoonForce(float dir, Vector2 oriForce)
    {
        float maxXForce = Mathf.Min(10f, oriForce.x);
        return new Vector2(0 * dir, 100);
    }

    public static Vector2 GetGroundSmoonForce(float dir,Vector2 oriForce)
    {
        float maxXForce = Mathf.Min(10f, oriForce.x);
        return new Vector2(maxXForce * dir, 20);
    }

    private static Regex m_RegexHPMoreThan = new Regex(@"(HPMoreThan:)([0-9]+)");
    private static Regex m_RegexHPLessThan = new Regex(@"(HPLessThan:)([0-9]+)");
}
