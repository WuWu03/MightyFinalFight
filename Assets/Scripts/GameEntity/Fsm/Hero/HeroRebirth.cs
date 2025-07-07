using GameFrameWork.FSM;
using GameFrameWork.Camera;
using UnityEngine;
using GameFrameWork;

public class HeroRebirth : BaseFsmState
{
    public Vector2 ReBirthPos
    {
        set
        {
            m_ReBirthPos = value;
        }
    }

    protected override void OnInit(FiniteStateMachine fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    protected override void OnEnter(FiniteStateMachine fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.onGroundEvent.AddListener(OnGround);
        m_Owner.SetDir(1);
        CameraMgr.instance.EndFollow();

        if (m_ReBirthPos == Vector2.zero)
        {
            Rect visionRect = CameraMgr.instance.GetVision();
            float rebirthPosX = visionRect.xMin + m_Owner.bound.size.x;
            float rebirthPosY = visionRect.yMax + m_Owner.bound.size.y;
            m_Owner.transform.localPosition = new Vector3(rebirthPosX, rebirthPosY, rebirthPosY);
        }
        else
        {
            m_Owner.transform.localPosition = new Vector3(m_ReBirthPos.x, m_ReBirthPos.y, m_ReBirthPos.y);
        }

        m_Owner.SetBodyType(RigidbodyType2D.Dynamic);
        m_Owner.PlayAnimation(AnimName.JumpDown);
        m_Owner.SetRebirthState();
    }

    protected override void OnSetStateData(BaseEventArgs stateData)
    {
        base.OnSetStateData(stateData);
    }

    protected override void OnExit(FiniteStateMachine fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.JumpUp);
        m_ReBirthPos = Vector2.zero;
    }

    protected override void OnRelease(FiniteStateMachine fsm)
    {
        m_Owner = null;
    }

    private void OnGround()
    {
        CameraMgr.instance.StartFollow();
    }

    private Vector2 m_ReBirthPos = Vector2.zero;
    private BaseHero m_Owner = null;
}