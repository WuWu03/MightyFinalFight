using UnityEngine;
using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;

public class DoRunToRoundPos : Action
{
    public DoRunToRoundPos(string name, string args, object owner) : base(name, args, owner)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
    }

    protected override void OnEnter()
    {
        Rect visionRect = CameraMgr.Ins.GetVision();
        Vector2 selfPos = m_Owner.Owner.Pos;
        Vector2 targetPos = PlayerMgr.Ins.Player.Pos;
        float randomY = StageMgr.Ins.GetRandomPosY(targetPos);
        float dir = targetPos.x > selfPos.x ? 1 : -1;
        float radius = Random.Range(0.2f, Vector2.Distance(selfPos, targetPos));
        Vector2 to = targetPos + Vector2.right * radius * dir;
        to.x = Mathf.Clamp(to.x, visionRect.xMin + m_Owner.Owner.Bound.width, visionRect.xMax - m_Owner.Owner.Bound.width);
        m_RoundPos[0].x = targetPos.x;
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
                return BehaviorTreeState.Success;
            }

            m_IsArravied = false;
            m_Owner.OppositePlayer();
            m_CurrIndex++;
        }

        m_IsArravied = Mathf.Abs(m_RoundPos[m_CurrIndex].x - m_Owner.Owner.Pos.x) <= 0.03f &&
                       Mathf.Abs(m_RoundPos[m_CurrIndex].y - m_Owner.Owner.Pos.y) <= 0.03f;

        if (!m_IsArravied)
        {
            m_Owner.Move((m_RoundPos[m_CurrIndex] - m_Owner.Owner.Pos).normalized, false);
            m_Owner.OppositePlayer();
        }

        return BehaviorTreeState.Running;
    }

    public override void Reset()
    {
        base.Reset();
        m_IsArravied = false;
    }

    private int m_CurrIndex = 0;
    private Vector2[] m_RoundPos = new Vector2[2] { Vector2.zero, Vector2.zero };
    private bool m_IsArravied = false;
    private new BaseEnemyCtrl m_Owner = null;
}