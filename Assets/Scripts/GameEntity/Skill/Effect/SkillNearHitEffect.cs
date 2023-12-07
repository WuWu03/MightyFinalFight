using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillConfigData;
using static UnityEngine.UI.GridLayoutGroup;

public class SkillNearHitEffect : SkillBaseEffect
{
    public SkillNearHitEffect(SkillConfigData m_SkillData, BaseRole owner, int effectIndex) : base(m_SkillData, owner, effectIndex)
    {
    }

    public override void Effect(ISkillSelector skillSelector)
    { 
        List<ICanBeHit> targets = m_Owner.OnHitStart();

        if (targets == null)
        {
            targets = skillSelector.GetTargets();
        }

        for (int i = 0; i < targets.Count; i++)
        {
            HurtData hurtData = SkillFactory.GetHurtData(targets[i], m_Owner, m_SkillData, m_SkillEffect);

            if (hurtData == null) 
            {
                continue;
            }

            bool isPause = false;// (m_Owner is BaseHero);

            if (!m_IsPause)
            {
                m_IsPause = isPause;
            }

            if (!m_IsHurtTarget)
            {
                m_IsHurtTarget = !targets[i].IsHurtWillDie(hurtData.attackValue);
            }

            if (isPause)
            {
                m_Owner.StartCoroutine(Pause(targets[i], hurtData, 0.2f, i, targets.Count));
            }
            else
            {
                targets[i].OnHurtMsg(hurtData);
            }
        }

        if (m_IsHurtTarget)
        {
            if (m_SkillEffect.IsShakeCamera)
            {
                CameraMgr.instance.Shake();
            }
        }

        m_Owner.OnHitEnd(m_SkillData, m_IsHurtTarget);

        if (!m_IsPause)
        {
            Complete();
        }
    }

    protected override void OnComplete()
    {
        base.OnComplete();
        m_IsHurtTarget = false;
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_IsHurtTarget = false;
        m_IsPause = false;
    }

    private IEnumerator Pause(ICanBeHit hit, HurtData hurtData, float duration, int targetIndex, int targetsCount)
    {
        yield return new WaitForSecondsRealtime(targetIndex * 0.2f);
        hit.OnHurtMsg(hurtData);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime((targetsCount > 1 ? targetsCount - 1 : 1) * duration);
        Time.timeScale = 1;

        if (targetIndex >=  targetsCount - 1)
        {
            Complete();
        }
    }

    private bool m_IsHurtTarget = false;
    private bool m_IsPause = false;
}