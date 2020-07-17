using System.Collections.Generic;
using UnityEngine;

public class SkillMoveHitEffect : SkillBaseEffect
{
    public SkillMoveHitEffect(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }
    public override bool IsCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }

    public override void Effect(ISkillSelector selector)
    {
        m_StartPos = base.m_Owner.transform.localPosition;
        m_OriginalGravity = base.m_Owner.Rigidbody.gravityScale;
        m_Selector = selector;
        m_IsCompleted = false;
        base.m_Owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        base.m_Owner.Rigidbody.AddForce(new Vector2(m_SkillEffect.AddSelfForce.x * base.m_Owner.Dir, m_SkillEffect.AddSelfForce.y));
        base.m_Owner.Rigidbody.drag = m_SkillEffect.AddSelfDrag;
        base.m_Owner.Rigidbody.gravityScale = m_SkillEffect.Gravity;
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

    public override void Reset()
    {
        m_IsCompleted = false;
    }

    public override void Exit()
    {
   
    }

    public override void Update()
    {
        m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.Pos.y);
        List<ICanBeHit> targets = m_Selector.GetTargets();
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
                    AttackForce = new Vector2(m_SkillEffect.AddTargetForce.x * m_Owner.Dir, m_SkillEffect.AddTargetForce.y),
                    IsSwoon = m_SkillEffect.IsSmoon,
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

        float dis = Mathf.Abs(m_SkillEffect.MoveDistance - Vector3.Distance(m_StartPos, m_Owner.transform.localPosition));
        if (dis <= 0.1f)
        {
            Complete();
            return;
        }
    }

    private Vector3 m_StartPos = Vector3.zero;
    private ISkillSelector m_Selector = null;
    private float m_OriginalGravity = 0;
}