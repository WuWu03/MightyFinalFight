using System;
using System.Collections.Generic;
using UnityEngine;

namespace WuWuFramework.Serialize
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_Keys = new();

        [SerializeField]
        private List<TValue> m_Values = new();
        
        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                m_Keys.Add(pair.Key);
                m_Values.Add(pair.Value);
            }
        }
        
        public void OnAfterDeserialize()
        {
            this.Clear();

            for (int i = 0; i < Mathf.Min(m_Keys.Count, m_Values.Count); i++)
            {
                this[m_Keys[i]] = m_Values[i];
            }
        }
    }
}