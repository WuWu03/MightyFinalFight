using FrameWork.GameEntity;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class BaseHero : BaseRole
    {
        class AttackerToSmoon
        {
            public BaseObject Attacker = null;
            public int HitTime = 0;
        }
        public override void Init(int id, string name)
        {
            base.Init(id,name);
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
            if(m_HitTime < 0)return;

            if(Time.time - m_HitTime > 0.5f)
            {
                m_DicAttacker.Clear();
                m_HitTime = -1;
            }
        }

        public override void OnHurtMsg(HurtData data)
        {
            int hitTime = 0;
            if(!m_DicAttacker.TryGetValue(data.AttackerID,out hitTime))
            {
                m_DicAttacker.Add(data.AttackerID,hitTime);
            }

            hitTime++;
            m_DicAttacker[data.AttackerID] = hitTime;

            if(hitTime >= 3)
            {
                data.IsSwoon = true;
                m_DicAttacker.Clear();
            }

            m_HitTime = Time.time;
            base.OnHurtMsg(data);
        }

        private float m_HitTime = -1f;
        private Dictionary<int,int> m_DicAttacker = null;
    }
}
