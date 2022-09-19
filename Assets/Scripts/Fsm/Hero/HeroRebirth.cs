using GameFrameWork.Fsm;
using GameFrameWork.Camera;
using UnityEngine;

public class HeroRebirth : BaseFsmState
{
    public Vector2 ReBirthPos
    {
        set
        {
            m_ReBirthPos = value;
        }
    }

    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseHero;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.ResetRigidbody();
        m_Owner.onGroundEvent.AddListener(OnGround);
        m_Owner.SetDir(1);
        CameraMgr.instance.EndFollow();

        if (m_ReBirthPos == Vector2.zero)
        {
            Rect visionRect = CameraMgr.instance.GetVision();
            float rebirthPosX = visionRect.xMin + m_Owner.boxCollider2D.size.x;
            float rebirthPosY = visionRect.yMax + m_Owner.boxCollider2D.size.y;
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

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.JumpUp);
        m_ReBirthPos = Vector2.zero;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private void OnGround()
    {
        CameraMgr.instance.StartFollow();
    }

    private Vector2 m_ReBirthPos = Vector2.zero;
    private BaseHero m_Owner = null;
}