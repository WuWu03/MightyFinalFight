using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Resources;
using WuWuFramework.Utils;

namespace WuWuFramework.BehaviourTree
{
    public class BehaviourTreeMgr : WuWuFrameworkModule, IBehaviourTreeMgr
    {
        private readonly Dictionary<object, BehaviourTree> m_BehaviourTrees;
        private readonly List<BehaviourTree> m_PersistentBehaviourTrees;
        private bool m_IsDirty;
        private IResourcesMgr m_ResourceMgr;

        public BehaviourTreeMgr()
        {
            m_BehaviourTrees = new();
            m_PersistentBehaviourTrees = new();
            MonoBehaviourMgr.instance.updateEvent += Update;
            MonoBehaviourMgr.instance.lateUpdateEvent += LateUpdate;
            MonoBehaviourMgr.instance.fixedUpdateEvent += FixedUpdate;
        }

        public void SetResourcesMgr(IResourcesMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
        }

        public void AddBehaviourTree(object owner, string dataName)
        {
            if (m_BehaviourTrees == null)
            {
                return;
            }

            string dataPath = PathUtil.FormatPath(WuWuFrameworkEntry.config.configDataPath, PathUtil.BehaviourTreeDataPath, dataName);
            byte[] buffer = m_ResourceMgr.Load<TextAsset>(dataPath).bytes;
            BehaviourTreeData data = new();
            data.DeSerialize(buffer);
            m_BehaviourTrees.Add(owner, new BehaviourTree(data, owner));
            m_IsDirty = true;
        }

        public void RemoveBehaviourTree(object owner)
        {
            if (m_BehaviourTrees.TryGetValue(owner, out BehaviourTree behaviourTree))
            {
                behaviourTree.Destroy();
                m_BehaviourTrees.Remove(owner);
                m_IsDirty = true;
            }
        }

        public void StartAllTrees()
        {
            foreach (var behaviourTree in m_BehaviourTrees)
            {
                behaviourTree.Value.Start();
            }
        }

        public void StartTree(object owner)
        {
            if (m_BehaviourTrees.TryGetValue(owner, out BehaviourTree behaviourTree))
            {
                behaviourTree.Start();
            }
        }

        public void StopAllTrees()
        {
            foreach (var behaviourTree in m_BehaviourTrees)
            {
                behaviourTree.Value.Stop();
            }
        }

        public void StopTree(object owner)
        {
            if (m_BehaviourTrees.TryGetValue(owner, out BehaviourTree behaviourTree))
            {
                behaviourTree.Stop();
            }
        }

        public void PauseAllTrees()
        {
            foreach (var behaviourTree in m_BehaviourTrees)
            {
                behaviourTree.Value.Pause();
            }
        }

        public void PauseTree(object owner)
        {
            if (m_BehaviourTrees.TryGetValue(owner, out BehaviourTree behaviourTree))
            {
                behaviourTree.Pause();
            }
        }

        public void ResumeAllTrees()
        {
            foreach (var behaviourTree in m_BehaviourTrees)
            {
                behaviourTree.Value.Resume();
            }
        }

        public void ResumeTree(object owner)
        {
            if (m_BehaviourTrees.TryGetValue(owner, out BehaviourTree behaviourTree))
            {
                behaviourTree.Resume();
            }
        }

        public override void Shutdown()
        {
            StopAllTrees();

            foreach (var behaviourTree in m_BehaviourTrees)
            {
                behaviourTree.Value.Destroy();
            }

            m_PersistentBehaviourTrees.Clear();
            m_BehaviourTrees.Clear();
            MonoBehaviourMgr.instance.updateEvent -= Update;
            MonoBehaviourMgr.instance.lateUpdateEvent -= LateUpdate;
            MonoBehaviourMgr.instance.fixedUpdateEvent -= FixedUpdate;
        }

        private void BuildPersistentTreesIfNeed()
        {
            if (!m_IsDirty)
            {
                return;
            }

            m_PersistentBehaviourTrees.Clear();
            m_PersistentBehaviourTrees.AddRange(m_BehaviourTrees.Values);
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            BuildPersistentTreesIfNeed();

            foreach (var behaviourTree in m_PersistentBehaviourTrees)
            {
                behaviourTree.Update(deltaTime);
            }
        }

        private void LateUpdate(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            BuildPersistentTreesIfNeed();

            foreach (var behaviourTree in m_PersistentBehaviourTrees)
            {
                behaviourTree.LateUpdate(deltaTime);
            }
        }

        private void FixedUpdate(float fixedDeltaTime, float fixedUnscaledDeltaTime, float fixedTime, float fixedUnscaledTime)
        {
            BuildPersistentTreesIfNeed();

            foreach (var behaviourTree in m_PersistentBehaviourTrees)
            {
                behaviourTree.FixedUpdate(fixedDeltaTime);
            }
        }
    }
}