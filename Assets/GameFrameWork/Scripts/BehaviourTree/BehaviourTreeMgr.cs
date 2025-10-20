using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTreeMgr : GameFrameWorkModule,IBehaviourTreeMgr
    {
        private readonly List<BehaviourTree> m_UsedBehaviourTreeList;
        private BehaviourTreeConfig m_Config;
        private IResourceMgr m_ResourceMgr;
        
        public BehaviourTreeMgr()
        {
            m_UsedBehaviourTreeList = new();
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Update(deltaTime);
            }
        }

        public override void LateUpdate(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].LateUpate(deltaTime);
            }
        }
        
        public override void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime, float fixedTime, float fixedUnscaledTime)
        {
            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].FixedUpdate(fixedDeltaTime);
            }
        }

        public override void Shutdown()
        {
            StopAllTrees();

            for (int i = m_UsedBehaviourTreeList.Count - 1; i > -1; i--)
            {
                m_UsedBehaviourTreeList[i].Destroy();
            }

            m_UsedBehaviourTreeList.Clear();
        }

        public void SetResourceMgr(IResourceMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
        }

        public void InitBehaviourTreeData()
        {
            string dataPath = PathUtil.FormatPath(GameFrameWorkEntry.config.configDataPath, PathUtil.behaviourTreeConfigDataName);
            string jsonStr = m_ResourceMgr.Load<TextAsset>(dataPath).text;
            m_Config = LitJson.JsonMapper.ToObject<BehaviourTreeConfig>(jsonStr);
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
    }
}