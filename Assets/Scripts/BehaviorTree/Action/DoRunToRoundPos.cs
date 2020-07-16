using UnityEngine;
using FrameWork.BehaviourTree;
using FrameWork.Camera;

public class DoRunToRoundPos : Action
{
    public DoRunToRoundPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        Vector2 enemyPos = m_Owner.Owner.Pos;
        Vector2 playerPos = PlayerMgr.Ins.Player.Pos;
        float randomY = StageMgr.Ins.GetRandomY(playerPos);
        float dir = playerPos.x > m_Owner.Owner.Pos.x ? 1 : -1;
        float radius = Random.Range(0.2f, Vector2.Distance(enemyPos, playerPos));
        Vector2 to = playerPos + Vector2.right * radius * dir;
        to.x = Mathf.Clamp(to.x, visionRect.xMin + m_Owner.Owner.Bound.width, visionRect.xMax - m_Owner.Owner.Bound.width);
        m_RoundPos[0].x = playerPos.x;
        m_RoundPos[0].y = randomY;
        m_RoundPos[1] = to;
        m_CurrIndex = 0;
        m_IsArravied = false;
    }

    public override BehaviorTreeState Excute()
    {
        if (m_IsArravied)
        {
            if (m_CurrIndex >= m_RoundPos.Length - 1)
            {
                m_Owner.Move(Vector2.zero, false);
                m_Owner.OppositePlayer();
                m_Owner.IsRoundPos = false;      
                return BehaviorTreeState.Success;
            }

            m_Owner.OppositePlayer();
            m_CurrIndex++;
        }

        Vector2 enemyPos = m_Owner.Owner.Pos;
        m_IsArravied = Mathf.Abs(m_RoundPos[m_CurrIndex].x - enemyPos.x) <= 0.05f &&
                       Mathf.Abs(m_RoundPos[m_CurrIndex].y - enemyPos.y) <= 0.05f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RoundPos[m_CurrIndex] - enemyPos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
        m_Owner.IsRoundPos = false;
    }

    private int m_CurrIndex = 0;
    private Vector2[] m_RoundPos = new Vector2[2] { Vector2.zero, Vector2.zero };
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}