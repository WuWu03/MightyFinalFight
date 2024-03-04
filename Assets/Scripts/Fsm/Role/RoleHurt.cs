using GameFrameWork.Fsm;

public class RoleHurt : BaseFsmState
{
    public string hurtAnim
    {
        set
        {
            m_HurtAnim = value;   
        }
    }

    protected override void OnInit(BaseFsm fsm)
    {
        m_Owner = fsm.owner as BaseRole;
    }

    protected override void OnEnter(BaseFsm fsm)
    {
        m_Owner.PlayAnimation(m_HurtAnim, 1, m_Owner.isBeCatch ? 1f : m_Owner.objectType == ObjectType.Player ? 0.5f : 1f);
        m_Owner.SetPos(m_Owner.pos, m_Owner.posZ);
    }

    protected override void OnUpdate(BaseFsm fsm, float deltaTime, float unscaleDeltaTime)
    {
        if (m_Owner.IsPlayComplete())
        {
            if (m_Owner.entityAttribute.health <= 0)
            {
                if (m_Owner.isInGround)
                {
                    ChangeState<RoleDead>(fsm);
                }
            }
            else ChangeState<RoleIdle>(fsm);
        }
    }

    protected override void OnExit(BaseFsm fsm, bool isShutdown)
    {
        m_Owner.StopAnimation(AnimName.Hurt);
        m_HurtAnim = string.Empty;
    }

    protected override void OnDestroy(BaseFsm fsm)
    {
        m_Owner = null;
    }

    private string m_HurtAnim;
    private BaseRole m_Owner = null;
}