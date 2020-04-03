using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using FrameWork.GameEntity;
using Runtime.Config;
using FrameWork.Camera;

namespace Runtime
{
    public class Trag :BaseObject
    {
        public override void Init(int id, string name)
        {
            base.Init(id, name);
            m_BoxCollider = gameObject.GetOrAddComponent<BoxCollider2D>();
            m_BoxCollider.enabled = true;
            m_BoxCollider.isTrigger = true;
        }

        public void SetTragData(SceneObjectData sceneObjectData)
        {     
            float width = (float)sceneObjectData.Area.Width / 100;
            float height = (float)sceneObjectData.Area.Height / 100;
            float posX = (float)sceneObjectData.Area.Pos.x / 100;
            float posY = (float)sceneObjectData.Area.Pos.y / 100;
            m_SceneObjectData = sceneObjectData;
            m_BoxCollider.size = new Vector2(width, height);
            SetPos2(posX, posY);
        }

        private void OnTriggerEnter2D(Collider2D collision)
       {
            BaseRole target = collision.gameObject.GetComponent<BaseRole>();
            if (target == null) return;

            Vector2 rebirthPos = Vector2.zero;
            float width = (float)m_SceneObjectData.Area.Width / 100;

            if (target.Pos.x < m_Pos.x)
                rebirthPos = new Vector2(m_Pos.x - width - 0.1f, target.Pos.y);
            else
                rebirthPos = new Vector2(m_Pos.x + width + 0.1f, target.Pos.y);

            target.OnDropMsg(new DropTragData() 
            {
                IsJustDead = target is BaseEnemy,
                InitPos = rebirthPos,
                AttackValue = 1,
            });
            
        }

        private SceneObjectData m_SceneObjectData = null;
        private BoxCollider2D m_BoxCollider = null;
    }
}