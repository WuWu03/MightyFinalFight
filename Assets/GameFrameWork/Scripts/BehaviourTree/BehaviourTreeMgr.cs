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
            m_ListBehaviourTrees = new List<BehaviourTree>();
            string dataPath = PathUtil.FormatPath(GameFrameWorkEntry.config.configDataPath, PathUtil.behaviourTreeConfigDataName);
            string jsonStr = AssetsMgr.instance.LoadAssetSync<TextAsset>(dataPath).text;
            m_Config = LitJson.JsonMapper.ToObject<BehaviourTreeConfig>(jsonStr);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();


            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].Update(Time.deltaTime);
            }
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();

            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].LateUpate(Time.deltaTime);
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].FixedUpdate(Time.fixedDeltaTime);
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            StopAllTrees();

            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].Destroy();
            }

            m_ListBehaviourTrees.Clear();
            m_ListBehaviourTrees = null;
        }

        public void AddBehaviourTree(object owner, int id)
        {
            if (m_ListBehaviourTrees == null)
            {
                return;
            }

            BehaviourTreeData data = m_Config.GetDataById(id);
            m_ListBehaviourTrees.Add(new BehaviourTree(data, owner));
        }

        public void RemoveBehaviourTree(object owner, int id)
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_ListBehaviourTrees[i];
                if (behaviourTree.tree.id == id && behaviourTree.tree.owner == owner)
                {
                    behaviourTree.Destroy();
                    m_ListBehaviourTrees.RemoveAt(i);
                    break;
                }
            }
        }

        public void StartAllTrees()
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].Start();
            }
        }

        public void StartTree(object owner, int id)
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_ListBehaviourTrees[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Start();
                    return;
                }
            }
        }

        public void StopAllTrees()
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].Stop();
            }
        }

        public void StopTree(object owner, int id)
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_ListBehaviourTrees[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Stop();
                    return;
                }
            }
        }

        public void PauseAllTrees()
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].Pause();
            }
        }

        public void PauseTree(object owner, int id)
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_ListBehaviourTrees[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Pause();
                    return;
                }
            }
        }

        public void ResumeAllTrees()
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                m_ListBehaviourTrees[i].Resume();
            }
        }

        public void ResumeTree(object owner, int id)
        {
            for (int i = m_ListBehaviourTrees.Count - 1; i > -1; i--)
            {
                BehaviourTree behaviourTree = m_ListBehaviourTrees[i];

                if (behaviourTree.tree.owner == owner && behaviourTree.tree.id == id)
                {
                    behaviourTree.Resume();
                    return;
                }
            }
        }

        private List<BehaviourTree> m_ListBehaviourTrees = null;
        private BehaviourTreeConfig m_Config = null;
    }
}