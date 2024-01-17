using GameFrameWork;
using System.Text.RegularExpressions;
using UnityEngine;
using static SkillConfigData;

public class SkillFactory
{
    public static SkillBaseDeployer CreateDeployer(int skillId, BaseRole owner)
    {
        SkillBaseDeployer deployer = null;
        SkillConfigData data = StaticConfig.SkillConfig.GetData(skillId);

        if (data == null)
        {
            Log.LogError("skill data is invalid skillId:", skillId);
            return null;
        }

        switch (data.DeployerType)
        {
            case SkillDeployerType.NormalAttack:
                deployer = new SkillNormalAttackDeployer(skillId, owner);
                break;
            case SkillDeployerType.JumpAttack:
                deployer = new SkillJumpAttackDeployer(skillId, owner);
                break;
            case SkillDeployerType.SkillAttack:
                deployer = new SkillSkillAttackDeployer(skillId, owner);
                break;
        }

        return deployer;
    }

    public static ISkillSelector[] CreateSelector(SkillConfigData skillData, BaseRole owner)
    {
        ISkillSelector[] selectors = new ISkillSelector[skillData.SkillEffects.Length];

        for (int i = 0; i < skillData.SkillEffects.Length; i++)
        {
            switch (skillData.SkillEffects[i].SelectorType)
            {
                case SkillSelectorType.None:
                    selectors[i] = new SkillNoneSelector(skillData, owner, i);
                    break;
                case SkillSelectorType.NearHitSelector:
                    selectors[i] = new SkillNearHitSelector(skillData, owner, i);
                    break;
                case SkillSelectorType.BulletSelector:
                    selectors[i] = new SkillBulletSelector(skillData, owner, i);
                    break;
            }
        }

        return selectors;
    }

    public static ISkillEffect[] CreateEffects(SkillConfigData skillData, BaseRole owner)
    {
        ISkillEffect[] effects = new ISkillEffect[skillData.SkillEffects.Length];

        for (int i = 0; i < skillData.SkillEffects.Length; i++)
        {
            switch (skillData.SkillEffects[i].EffectorType)
            {
                case SkillEffectorType.None:
                    effects[i] = new SkillNoneEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.NearHitEffect:
                    effects[i] = new SkillNearHitEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.BulletHitEffect:
                    effects[i] = new SkillBulletEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.MoveHitEffect:
                    effects[i] = new SkillMoveHitEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.TargetPositionEffect:
                    effects[i] = new SkillTargetTransformEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.TargetScaleEffect:
                    effects[i] = new SkillTargetTransformEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.SelfTransformEffect:
                    effects[i] = new SkillSelfTransformEffect(skillData, owner, i);
                    break;
                case SkillEffectorType.SubHP:
                    effects[i] = new SkillSubHPEffect(skillData, owner, i);
                    break;
            }
        }

        return effects;
    }
}