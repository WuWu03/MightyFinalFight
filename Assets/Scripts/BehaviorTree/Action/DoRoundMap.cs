using GameFrameWork.BehaviourTree;
using GameFrameWork.Camera;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DoRoundMap : Action
{
    public DoRoundMap(string name, string args, object owner, int priority) : base(name, args, owner, priority)
    {
        m_Owner = base.m_Owner as BaseEnemyCtrl;
        m_ListPos = new List<Vector2>();
    }

    protected override void OnEnter()
    {
        m_State = BehaviourTreeState.Running;

        m_ListPos.Clear();
        m_CurrIndex = 0;
        m_MoveTimer = -1;
        m_MoveTime = Random.Range(4f, 8f);

        float tirggerSize = m_Owner.owner.GetCurrTriggerSize().x / 2f;

        Vector2 pos = m_Owner.owner.pos;
        Rect vision = CameraMgr.instance.GetVision();
        Rect area = StageMgr.instance.GetMoveArea();

        Vector2 leftTop = new Vector2(vision.xMin + tirggerSize, area.yMax);
        Vector2 leftBottom = new Vector2(vision.xMin + tirggerSize, area.yMin);
        Vector2 rightTop = new Vector2(vision.xMax - tirggerSize, area.yMax);
        Vector2 rightBottom = new Vector2(vision.xMax - tirggerSize, area.yMin);

        float leftDistance = Mathf.Abs(pos.x - vision.xMin);
        float rightDistance = Mathf.Abs(pos.x - vision.xMax);
        float topDistance = Mathf.Abs(pos.y - area.yMax);
        float bottomDistance = Mathf.Abs(pos.y - area.yMin);
        bool isLeft = leftDistance < rightDistance;
        bool isTop = topDistance < bottomDistance;

        if (isLeft)
        {
            if (isTop) CreatePosList(leftTop, rightTop, rightBottom, leftBottom);
            else CreatePosList(leftBottom, rightBottom, rightTop, leftTop);
        }
        else
        {
            if (isTop) CreatePosList(rightTop, leftTop, leftBottom, rightBottom);
            else CreatePosList(rightBottom, leftBottom, leftTop, rightTop);
        }
    }

    public override bool CanExcute()
    {
        return m_State != BehaviourTreeState.Success;
    }

    public override BehaviourTreeState Excute()
    {
        return m_State;
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (m_MoveTimer < 0)
        {
            m_MoveTimer = Time.time;
        }

        if (Time.time - m_MoveTimer >= m_MoveTime)
        {
            m_State = BehaviourTreeState.Success;
            return;
        }

        bool isArrive = Vector2.Distance(m_ListPos[m_CurrIndex], m_Owner.owner.pos) <= 0.02f;

        if (isArrive)
        {
            m_CurrIndex++;

            if (m_CurrIndex >= m_ListPos.Count)
            {
                m_CurrIndex = 0;
            }
        }
        else
        {
            m_Owner.Move((m_ListPos[m_CurrIndex] - m_Owner.owner.pos).normalized, false);
            m_Owner.OppositePlayer();
        }
    }

    protected override void OnReset()
    {
        base.OnReset();
        m_ListPos.Clear();
        m_CurrIndex = 0;
        m_MoveTime = 0;
        m_MoveTimer = -1;
    }

    private void CreatePosList(Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4)
    {
        m_ListPos.Add(pos1);
        m_ListPos.Add(pos2);
        m_ListPos.Add(pos3);
        m_ListPos.Add(pos4);
    }

    private float m_MoveTimer = -1;
    private float m_MoveTime = 0;
    private int m_CurrIndex = 0;
    private List<Vector2> m_ListPos = null;
    private BehaviourTreeState m_State = BehaviourTreeState.None;
    protected new BaseEnemyCtrl m_Owner = null;
}