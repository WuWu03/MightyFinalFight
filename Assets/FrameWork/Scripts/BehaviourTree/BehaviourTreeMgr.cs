using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FrameWork.BehaviourTree
{
    public class BehaviourTreeMgr
    {
        public BehaviourTreeMgr(object owner, BehaviourTreeConfig config)
        {
            m_Owner = owner;
            if(m_Config == null)
            {
                m_Config = config;
            }
        }

        public void Init(params int[] treesID)
        {
            if(m_Config == null)
            {
                Debugger.LogError("Behavior tree data not found, generate behavior tree data please!");
                return;
            }

            m_ListTrees = new List<BehaviourTree>();
            m_ListTreeID = new List<int>();
            m_ListTreeID.AddRange(treesID);
            LoadAll();
        }

        public void AddBehaviourTree(int id)
        {
            if (m_ListTreeID == null) return;
            m_ListTreeID.Add(id);
            BehaviourTreeData data = m_Config.GetData(id);
            m_ListTrees.Add(new BehaviourTree(data, m_Owner));
        }

        public void Start(int index = 0)
        {
            for(int  i = 0; i < m_ListTrees.Count; i++)
            {
                m_ListTrees[i].Stop();
            }

            m_ListTrees[index].Start();
        }

        public void Update(float deltaTime)
        {
            for(int  i = 0; i < m_ListTrees.Count; i++)
            {
                m_ListTrees[i].Update(deltaTime);
            }
        }

        public void StopAll()
        {
            for (int i = 0; i < m_ListTrees.Count; i++)
            {
                Stop(i);
            }
        }

        public void Stop(int index)
        {
            if (index < 0 || index >= m_ListTrees.Count) return;
            m_ListTrees[index].Stop();
        }

        public void ShutDown()
        {
            StopAll();
            m_ListTrees.Clear();
            m_ListTreeID.Clear();
        }

        private void LoadAll()
        {
            for (int i = 0; i < m_ListTreeID.Count; i++)
            {
                BehaviourTreeData data = m_Config.GetData(m_ListTreeID[i]);
                m_ListTrees.Add(new BehaviourTree(data, m_Owner));
            }
        }

        private List<BehaviourTree> m_ListTrees = null;
        private object m_Owner = null;
        private List<int> m_ListTreeID = null;
        private static BehaviourTreeConfig m_Config = null;
    }
}