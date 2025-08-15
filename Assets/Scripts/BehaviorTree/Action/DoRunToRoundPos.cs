using UnityEngine;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;

public class DoRunToRoundPos : Action
{
    public DoRunToRoundPos(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
    {
        m_Owner = base.m_Owner as BaseEnemy;
    }

    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnEnter()
    {
        Rect visionRect = CameraMgr.instance.GetVision();
        Vector2 selfPos = m_Owner.pos;
        Vector2 targetPos = PlayerMgr.instance.player.pos;
        float randomY = StageMgr.instance.GetRandomPosY();
        float dir = targetPos.x > selfPos.x ? 1 : -1;
        float radius = Random.Range(0.2f, Vector2.Distance(selfPos, targetPos));
        Vector2 to = targetPos + Vector2.right * radius * dir;
        to.x = Mathf.Clamp(to.x, visionRect.xMin + m_Owner.bound.width, visionRect.xMax - m_Owner.bound.width);
        m_RoundPos[0].x = targetPos.x;
        m_RoundPos[0].y = randomY;
        m_RoundPos[1] = to;
        m_CurrIndex = 0;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        bool isArravied = Mathf.Abs(m_RoundPos[m_CurrIndex].x - m_Owner.pos.x) <= 0.03f && Mathf.Abs(m_RoundPos[m_CurrIndex].y - m_Owner.pos.y) <= 0.03f;

        if (isArravied)
        {
            if (m_CurrIndex >= m_RoundPos.Length - 1)
            {
                m_Owner.Move(Vector2.zero, false);
                m_Owner.OppositePlayer();
                m_State = BehaviourTreeState.Success;
                return;
            }

            m_Owner.OppositePlayer();
            m_CurrIndex++;
        }
        else
        {
            m_Owner.Move((m_RoundPos[m_CurrIndex] - m_Owner.pos).normalized, false);
            m_Owner.OppositePlayer();
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_CurrIndex = 0;
    }

    private int m_CurrIndex = 0;
    private Vector2[] m_RoundPos = new Vector2[2] { Vector2.zero, Vector2.zero };
    private new BaseEnemy m_Owner = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
}