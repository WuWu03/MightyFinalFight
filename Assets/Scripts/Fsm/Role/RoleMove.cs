using GameFrameWork.Camera;
using GameFrameWork.Fsm;
using UnityEngine;

public class RoleMove : BaseFsmState
{
    public bool canChangeDir
    {
        set
        {
            m_CanChangeDir = value;
        }
    }

    public bool isCatch
    {
        set
        {
            m_IsCatch = value;   
        }
    }

    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        if (m_Owner.objectType == ObjectType.Player && (m_Owner as BaseHero).weapon != null)
        {
            m_Owner.PlayAnimation(m_IsCatch ? AnimName.Move_Catch : AnimName.Move_Weapon, 0, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
        else
        {
            m_Owner.PlayAnimation(m_IsCatch ? AnimName.Move_Catch : AnimName.Move, 0, m_Owner.entityAttribute.moveSpeed * 0.2f);
        }
    }

    protected override void OnFixedUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_CanChangeDir && !m_IsCatch)
        {
            m_Owner.SetDir(m_Owner.moveDir.x);
        }

        Vector3 ownerPos = m_Owner.transform.localPosition + new Vector3(m_Owner.moveDir.x, m_Owner.moveDir.y) * m_Owner.entityAttribute.moveSpeed * Time.deltaTime;
        m_Owner.SetPos2(ownerPos);
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Move_Catch);
        m_Owner.StopAnimation(AnimName.Move);
        m_Owner.StopAnimation(AnimName.Move_Weapon);
        m_CanChangeDir = false;
        m_IsCatch = false;
    }

    private bool m_CanChangeDir = false;
    private bool m_IsCatch = false;
    private BaseRole m_Owner = null;
}