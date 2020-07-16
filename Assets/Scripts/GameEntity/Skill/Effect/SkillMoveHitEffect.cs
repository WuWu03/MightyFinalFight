using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillMoveHitEffect : ISkillEffect
{
    public bool IsCompleted
    {
        get
        {
            if (!m_IsCompleted)
            {
                OnUpdate();
            }
            return m_IsCompleted;
        }
    }

    public int Index
    {
        get;
        set;
    }

    public void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector)
    {
        m_Owner = owner;
        m_SkillData = skillData;
        m_StartPos = owner.transform.localPosition;
        m_OriginalGravity = owner.Rigidbody.gravityScale;
        m_Selector = selector;
        m_IsCompleted = false;
        owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        owner.Rigidbody.AddForce(new Vector2(skillData.SkillEffects[Index].AddSelfForce.x * owner.Dir, skillData.SkillEffects[Index].AddSelfForce.y));
        owner.Rigidbody.drag = skillData.SkillEffects[Index].AddSelfDrag;
        owner.Rigidbody.gravityScale = skillData.SkillEffects[Index].Gravity;
    }

    private void OnUpdate()
    {
        if (m_Owner == null) return;
        m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);
        List<ICanBeHit> targets = m_Selector.GetTargets(m_Owner, m_SkillData);
        bool hasHit = false;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].CanBeHit)
            {
                targets[i].OnHurtMsg(new HurtData()
                {
                    AttackerID = m_Owner.ID,
                    AttackerDir = m_Owner.Dir,
                    AttackerPos = m_Owner.Pos,
                    AttackForce = new Vector2(m_SkillData.SkillEffects[Index].AddTargetForce.x * m_Owner.Dir, m_SkillData.SkillEffects[Index].AddTargetForce.y),
                    IsSwoon = m_SkillData.SkillEffects[Index].IsSmoon,
                    AttackValue = 1,
                });

                hasHit = true;
            }
        }

        if (hasHit)
        {
            Complete();
            return;
        }

        float dis = Mathf.Abs(m_SkillData.SkillEffects[Index].MoveDistance - Vector3.Distance(m_StartPos, m_Owner.transform.localPosition));
        if (dis <= 0.1f)
        {
            Complete();
            return;
        }
    }

    private void Complete()
    {
        //m_Owner.Rigidbody.velocity = Vector2.zero;
        //m_Owner.Rigidbody.gravityScale = m_OriginalGravity;
        //m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_IsCompleted = true;
        m_SkillData = null;
        m_Owner = null;
        m_Selector = null;
    }

    public void Reset()
    {
        m_IsCompleted = false;
    }

    public void Exit()
    {
   
    }

    private bool m_IsCompleted = false;
    private SkillData m_SkillData = null;
    private BaseRole m_Owner = null;
    private Vector3 m_StartPos = Vector3.zero;
    private ISkillSelector m_Selector = null;
    private float m_OriginalGravity = 0;
}