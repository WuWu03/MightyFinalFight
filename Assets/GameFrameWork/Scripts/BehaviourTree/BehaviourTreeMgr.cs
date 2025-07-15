using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTreeMgr : BaseMgr<BehaviourTreeMgr>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            m_UsedBehaviourTreeList = new List<BehaviourTree>();
            m_UnUsedBehaviourTreeList = new List<BehaviourTree>();
            string dataPath = PathUtil.FormatPath(GameFrameWorkEntry.config.configDataPath, PathUtil.behaviourTreeConfigDataName);
            string jsonStr = AssetsMgr.instance.LoadAssetSync<TextAsset>(dataPath).text;
            m_Config = LitJson.JsonMapper.ToObject<BehaviourTreeConfig>(jsonStr);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();


            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Update(Time.deltaTime);
            }
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();

            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].LateUpate(Time.deltaTime);
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].FixedUpdate(Time.fixedDeltaTime);
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            StopAllTrees();

            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Destroy();
            }

            m_UsedBehaviourTreeList.Clear();
            m_UsedBehaviourTreeList = null;
        }

        public void AddBehaviourTree(object owner, int id)
        {
            if (m_UsedBehaviourTreeList == null)
            {
                return;
            }

            BehaviourTreeData data = m_Config.GetDataById(id);
            m_UsedBehaviourTreeList.Add(new BehaviourTree(data, owner));
        }

        public void RemoveBehaviourTree(object owner, int id)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_UsedBehaviourTreeList[i];
                if (behaviourTree.tree.id == id && behaviourTree.tree.owner == owner)
                {
                    behaviourTree.Destroy();
                    m_UsedBehaviourTreeList.RemoveAt(i);
                    break;
                }
            }
        }

        public void StartAllTrees()
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Start();
            }
        }

        public void StartTree(object owner, int id)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_UsedBehaviourTreeList[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Start();
                    return;
                }
            }
        }

        public void StopAllTrees()
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Stop();
            }
        }

        public void StopTree(object owner, int id)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_UsedBehaviourTreeList[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Stop();
                    return;
                }
            }
        }

        public void PauseAllTrees()
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Pause();
            }
        }

        public void PauseTree(object owner, int id)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_UsedBehaviourTreeList[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Pause();
                    return;
                }
            }
        }

        public void ResumeAllTrees()
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Resume();
            }
        }

        public void ResumeTree(object owner, int id)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_UsedBehaviourTreeList[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Resume();
                    return;
                }
            }
        }

        private List<BehaviourTree> m_UsedBehaviourTreeList = null;
        private List<BehaviourTree> m_UnUsedBehaviourTreeList = null;
        private BehaviourTreeConfig m_Config = null;
    }
}