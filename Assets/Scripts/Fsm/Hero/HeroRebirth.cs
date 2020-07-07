using FrameWork.Fsm;
using FrameWork.Camera;
using UnityEngine;

public class HeroRebirth : BaseFsmState
{
    public Vector2 ReBirthPos = Vector2.zero;
    public override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.Owner as BaseRole;
    }

    public override void OnEnter(BaseFsm fsm)
    {
        m_Owner.OnGroundEvent.AddListener(OnGround);
        CameraMgr.Ins.EndFollow();
       
        if (ReBirthPos == Vector2.zero)
        {
            Vector2[] vision = CameraMgr.Ins.GetVision();
            float rebirthPosX = vision[0].x + m_Owner.Collider.size.x;
            float rebirthPosY = vision[1].y + m_Owner.Collider.size.y;
            m_Owner.transform.localPosition = new Vector3(rebirthPosX, rebirthPosY, rebirthPosY);
        }
        else
        {
            m_Owner.transform.localPosition = new Vector3(ReBirthPos.x, ReBirthPos.y, ReBirthPos.y);
        }
        m_Owner.Rigidbody.gravityScale = 1;
        m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Owner.PlayAnimation(AnimName.JumpDown);
    }

    public override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
    }

    public override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.JumpUp);
        ReBirthPos = Vector2.zero;
    }

    public override void OnDestroy(BaseFsm fsm)
    {

    }

    private void OnGround()
    {
        CameraMgr.Ins.StartFollow();
    }

    private BaseRole m_Owner = null;
}