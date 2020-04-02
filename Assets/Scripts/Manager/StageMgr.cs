using FrameWork;
using FrameWork.Camera;
using FrameWork.Pool;
using FrameWork.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class StageMgr : BaseMgr<StageMgr>
    {
        public int CurrID
        {
            get
            {
                return m_CurrID;
            }
        }

        public int Width
        {
            get
            {
                return m_Width;
            }
        }
        public int Heigth
        {
            get
            {
                return m_Height;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            if (m_MapRenderer == null)
            {
                m_MapRenderer = new GameObject("Map").GetOrAddComponent<SpriteRenderer>();
                m_MapRenderer.transform.SetParent(transform, false);
                Utils.SetLayer(m_MapRenderer.gameObject, LayerMask.NameToLayer("Map"), true);
                DontDestroyOnLoad(m_MapRenderer.gameObject);
            }
        }


        public void Enter(int id)
        {
            if (CurrID.Equals(id)) return;
            m_CurrID = id;
            m_CurrStageData = StaticConfig.StageConfig.GetData(id);
            m_Width = m_CurrStageData.Width;
            m_Height = m_CurrStageData.Height;
            m_CurrAreaIndex = 0;

            float x = -1f;
            for (int i = 0; i < 5; i++)
            {
                x += 0.2f * (float)i;
                BaseEnemy enemy = ObjectPool.Ins.Get<BaseEnemy>("Monster");
                enemy.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, "Cody"));
                enemy.SetObjectType(ObjectType.Monster);
                enemy.SetPos2(x, -0.35f);
            }

            CameraMgr.Ins.EndFollow();
            string resPath = ResDefine.TEX_PATH + m_CurrStageData.AssetName;
            ResMgr.Ins.LoadAsset(resPath, OnLoadComplete, true, typeof(Sprite));
        }

        public bool IsOutArea(Vector2 pos)
        {
            if (m_CurrStageData.Areas == null || m_CurrStageData.Areas.Length < 1)
            {
                return false;
            }
            pos = pos * 100;

            return IsInArea(m_CurrStageData.Areas[m_CurrAreaIndex], pos);
        }

        public bool CanMove(Vector2 pos)
        {
            pos = pos * 100;
            for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
            {
                if(IsInArea(m_CurrStageData.MoveArea[i],pos))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsInArea(StageData.Area area, Vector2 pos)
        {
            int xLeft = area.Pos.x - area.Width / 2;
            int xRigth = area.Pos.x + area.Width / 2;
            int yLeft = area.Pos.y - area.Height / 2;
            int yRigth = area.Pos.y + area.Height / 2;

            if (pos.x > xLeft && pos.x < xRigth && pos.y > yLeft && pos.y < yRigth)
            {
                return true;
            }

            return false;
        }

        public override void ShutDown()
        {

        }

        private void OnLoadComplete(UnityEngine.Object obj)
        {
            Sprite sprite = obj as Sprite;
            m_MapRenderer.sprite = sprite;
 
            PlayerMgr.Ins.Player.SetPos(m_CurrStageData.InitPos);
            CameraMgr.Ins.StartFollow(m_CurrStageData.Width, m_CurrStageData.Height);
        }

        private int m_Width;
        private int m_Height;
        private SpriteRenderer m_MapRenderer = null;
        private StageData m_CurrStageData = null;
        private int m_CurrID = 0;
        private int m_CurrAreaIndex = 1;
    }
}
