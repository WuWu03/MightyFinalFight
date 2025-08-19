using GameFrameWork.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillNearHitEffect : SkillBaseEffect
{
    public SkillNearHitEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector skillSelector)
    {
        m_Targets = m_Owner.OnHitStart();

        if (m_Targets == null || m_Targets.Count < 1)
        {
            m_Targets = skillSelector.GetTargets();
        }

        if (m_Targets == null || m_Targets.Count < 1)
        {
            Complete();
            return;
        }

        m_IsPause = m_SkillData.id == 1001004;
        m_Owner.StartCoroutine(Pause());
    }

    private IEnumerator Pause()
    {
        for (int i = 0; i < m_Targets.Count; i++)
        {
            HurtTarget(m_Targets[i]);

            if(m_IsPause)
            {
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(0.2f);
                Time.timeScale = 1f;
            }
        }

        Time.timeScale = 1f;

        if (m_IsHurtTarget)
        {
            if (m_SkillEffect.IsShakeCamera)
            {
                CameraMgr.instance.Shake(0.3f, 0.1f, 20, 100);
            }
        }

        m_Owner.OnHitEnd(m_SkillData, m_IsHurtTarget);
        Complete();
    }

    protected override void OnComplete()
    {
        base.OnComplete();
        m_IsHurtTarget = false;
        m_IsPause = false;
        m_Owner.StopCoroutine(Pause());
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_IsHurtTarget = false;
        m_IsPause = false;
        m_Targets = null;
    }

    private bool HurtTarget(ICanBeHit canBeHit)
    {
        if (SkillUtil.SkillHit(canBeHit, m_Owner, m_SkillData, m_SkillEffect))
        {
            m_IsHurtTarget = true;
            return true;
        }

        return false;
    }

    private bool m_IsHurtTarget = false;
    private bool m_IsPause = false;
    private List<ICanBeHit> m_Targets = null;
}