using DG.Tweening.Core;
using GameFrameWork.Assets;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.BehaviourTree
{
    public class BehaviourTreeMgr
    {
        public static void Init(string configPath)
        {
            if (m_Config == null)
            {
                string jsonStr = AssetsMgr.instance.LoadAssetSync<TextAsset>(configPath).text;
                m_Config = LitJson.JsonMapper.ToObject<BehaviourTreeConfig>(jsonStr);
            }
        }

        public BehaviourTreeMgr(object owner)
        {
            m_Owner = owner;
        }

        public void InitTree(params int[] treesID)
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
            if (m_ListTreeID == null)
            {
                return;
            }

            m_ListTreeID.Add(id);
            BehaviourTreeData data = m_Config.GetDataById(id);
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
            if (index < 0 || index >= m_ListTrees.Count)
            {
                return;
            }

            m_ListTrees[index].Stop();
        }

        public void ShutDown()
        {
            StopAll();

            for (int i = 0; i < m_ListTrees.Count; i++)
            {
                m_ListTrees[i].Destroy();
            }

            m_ListTrees.Clear();
            m_ListTreeID.Clear();
            m_ListTrees = null;
            m_ListTreeID = null;
            m_Owner = null;
        }

        private void LoadAll()
        {
            for (int i = 0; i < m_ListTreeID.Count; i++)
            {
                BehaviourTreeData data = m_Config.GetDataById(m_ListTreeID[i]);
                m_ListTrees.Add(new BehaviourTree(data, m_Owner));
            }
        }

        private List<BehaviourTree> m_ListTrees = null;
        private List<int> m_ListTreeID = null;
        private object m_Owner = null;
        private static BehaviourTreeConfig m_Config = null;
    }
}