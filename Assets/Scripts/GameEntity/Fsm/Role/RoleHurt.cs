using GameFrameWork;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleHurt : BaseFsmState
{
    protected override void OnInit(Fsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(Fsm fsm)
    {
        m_Owner.SetCanAttack(false);
        m_Owner.SetCanBeHit(true);
        m_Owner.SetCanJump(false);
        m_Owner.SetCanMove(false);
        m_Owner.SetCanSkill(false);
        m_Owner.SetCanBeCatch(false);
        m_Owner.PlayAnimation(m_HurtAnim, 1, m_Owner.isBeCatch ? 1f : m_Owner.objectType == ObjectType.Player ? 0.5f : 1f);
        m_Owner.SetPos(m_Owner.pos, m_Owner.posZ);
    }

    protected override void OnUpdate(Fsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsCurrAnimationComplete())
        {
            if (m_HurtTimer < 0)
            {
                m_HurtTimer = Time.time + (m_Owner.objectType == ObjectType.Player ? 0f : -0.3f);
            }
        }

        if (m_HurtTimer > 0 && Time.time - m_HurtTimer >= 0.2f)
        {
            ChangeState<RoleIdle>(fsm);
        }
    }

    protected override void OnSetStateData(Fsm fsm, BaseEventArgs stateData)
    {
        base.OnSetStateData(fsm, stateData);
        HurtStateData hurtData = stateData as HurtStateData;
        m_HurtAnim = hurtData.hurtAnim;
    }

    protected override void OnExit(Fsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Hurt);
        m_HurtTimer = -1f;
        m_HurtAnim = string.Empty;
    }

    protected override void OnRelease(Fsm fsm)
    {
        m_Owner = null;
    }

    private string m_HurtAnim = string.Empty;
    private float m_HurtTimer = -1f;
    private BaseRole m_Owner = null;
}