using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public class SkillMoveHitEffect : ISkillEffect
    {
        public bool IsCompleted
        {
            get
            {
                if(!m_IsCompleted)
                {
                    OnUpdate();
                }
                return m_IsCompleted;
            }
        }

        public void Effect(BaseAvatar owner, SkillData skillData, ISkillSelector selector)
        {
            m_Owner = owner;
            m_SkillData = skillData;     
            m_StartPos = owner.transform.localPosition;
            m_OriginalGravity = owner.Rigidbody.gravityScale;
            m_Selector = selector;
            m_IsCompleted = false;

            owner.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            owner.Rigidbody.AddForce(new Vector2(skillData.AddSelfForce.x * owner.Dir, skillData.AddSelfForce.y));
            owner.Rigidbody.drag = skillData.AddSelfDrag;    
            owner.Rigidbody.gravityScale = skillData.Gravity;
        }

        private void OnUpdate()
        {
            m_Owner.UpdatePos2(m_Owner.transform.localPosition.x, m_Owner.transform.localPosition.y);
            List<GameObject> targets = m_Selector.GetTargets(m_Owner, m_SkillData);
            bool hasHit = false;

            for (int i = 0; i < targets.Count; i++)
            {
                ICanBeHit hit = targets[i].GetComponent<ICanBeHit>();
                if (hit != null && hit.CanBeHit)
                {
                    hit.OnHurtMsg(new HurtData()
                    {
                        AttackForce = new Vector2(m_SkillData.AddTargetForce.x * m_Owner.Dir, m_SkillData.AddTargetForce.y),
                        IsSwoon = m_SkillData.IsSmoon,
                        AttackValue = 1,
                    });

                    hasHit = true;
                }
            }

            if(hasHit)
            {
                Reset();
                return;
            }

            float dis = Mathf.Abs(m_SkillData.MoveDistance - Vector3.Distance(m_StartPos, m_Owner.transform.localPosition));
            if (dis <= 0.1f)
            {
                Reset();
                return;
            }
        }

        private void Reset()
        {
            m_Owner.Rigidbody.velocity = Vector2.zero;
            m_Owner.Rigidbody.gravityScale = m_OriginalGravity;
            m_Owner.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            m_IsCompleted = true;
            m_SkillData = null;
            m_Owner = null;
            m_Selector = null;
        }


        private bool m_IsCompleted = false;
        private SkillData m_SkillData = null;
        private BaseAvatar m_Owner = null;
        private Vector3 m_StartPos = Vector3.zero;
        private ISkillSelector m_Selector = null;
        private float m_OriginalGravity = 0;
    }
}
