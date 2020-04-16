using FrameWork.GameEntity;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseRole
{
    public override bool CanMove
    {
        get
        {
            return base.CanMove && m_CatchTarget == null;
        }
    }

    public override bool CanSkill
    {
        get
        {
            return base.CanSkill && m_CatchTarget == null;
        }
    }
    
    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<HeroRebirth>();
        m_DicAttacker = new Dictionary<int, int>();
    }

    public override void Release()
    {
        base.Release();
        m_DicAttacker.Clear();
        m_DicAttacker = null;
    }

    protected override void Update()
    {
        base.Update();
        CheckCatch();

        if (m_HitTime < 0) return;

        if (Time.time - m_HitTime > 1.0f)
        {
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
    }

    public override void OnAttackMsg(AttackData data)
    {
        base.OnAttackMsg(data);
    }

    public override void OnHurtMsg(HurtData data)
    {
        if (data.IsSwoon)
        {
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
        else
        {
            int hitTime = 0;
            if (!m_DicAttacker.TryGetValue(data.AttackerID, out hitTime))
            {
                m_DicAttacker.Add(data.AttackerID, hitTime);
            }

            hitTime++;
            m_DicAttacker[data.AttackerID] = hitTime;

            if (hitTime >= 3)
            {
                data.AttackForce = new Vector2(40 * data.AttackerDir, 120);
                data.IsSwoon = true;
                m_DicAttacker.Clear();
            }
            m_HitTime = Time.time;
        }

        base.OnHurtMsg(data);
    }

    public void OnRebirthMsg()
    {
        ChangeState<HeroRebirth>();
    }

    protected virtual void CheckCatch()
    {    
        if (m_CatchTarget == null)
        {
            if (!IsAnyState(typeof(RoleMove)) || m_TriggerTargets.Targets.Count < 1) return;

            for (int i = 0; i < m_TriggerTargets.Targets.Count; i++)
            {
                ICanBeHit temp = m_TriggerTargets.Targets[i].GetComponent<ICanBeHit>();
                if (temp == null || !temp.CanBeHit) continue;
                BaseObject targetObj = m_TriggerTargets.Targets[i].GetComponent<BaseObject>();
                bool isInRange = Mathf.Abs(targetObj.Pos.y - m_Pos.y) <= 0.05f &&
                                 Mathf.Abs(targetObj.Pos.x - m_Pos.x) <= 0.15f &&
                                    (targetObj.Pos.x - m_Pos.x) * m_Dir > 0;
                if (isInRange)
                {
                    targetObj.SetDir(m_Dir * -1);
                    m_CatchTarget = temp;
                    break;
                }
            }

            if(m_CatchTarget != null)
            {
                m_CatchStamp = Time.time;
            }

            return;
        }

        if (Time.time - m_CatchStamp >= m_CatchTime)
        {
            m_CatchTarget = null;
            m_CatchStamp = 0f;
            ChangeState<RoleIdle>();
            return;
        }
    }

    private ICanBeHit m_CatchTarget = null;
    private float m_CatchStamp = 0f;
    protected float m_CatchTime = 2f;
    private float m_HitTime = -1f;
    private Dictionary<int, int> m_DicAttacker = null;
}