using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Serialize
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [Serializable]
        struct TempKeyValuePair
        {
            public TKey Key;
            public TValue Value;
        }

        public TValue this[TKey key]
        {
            get
            {
                return m_Target[key];
            }
            set
            {
                m_Target[key] = value;
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                return m_Target.Keys;
            }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                return m_Target.Values;
            }
        }

        public int Count
        {
            get
            {
                return m_Target.Count;
            }
        }

        public Dictionary<TKey, TValue> ToDictionary() 
        {
            return m_Target; 
        }

        public SerializableDictionary()
        {
            m_Target = new Dictionary<TKey, TValue>();
            m_ListValuePair = new List<TempKeyValuePair>();
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return m_Target.GetEnumerator();
        }

        public void Add(TKey key, TValue value)
        {
            m_Target.Add(key, value);
        }

        public void Remove(TKey key)
        {
            m_Target.Remove(key);
        }

        public void Clear()
        {
            m_Target.Clear();
        }

        public bool Contains(TKey key)
        {
            return m_Target.ContainsKey(key);
        }

        public bool ContainsKey(TKey key)
        {
            return m_Target.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Target.TryGetValue(key, out value);
        }

        public void OnBeforeSerialize()
        {
            m_ListValuePair.Clear();

            foreach (KeyValuePair<TKey, TValue> item in m_Target)
            {
                m_ListValuePair.Add(new TempKeyValuePair()
                {
                    Key = item.Key,
                    Value = item.Value,
                });
            }
        }

        public void OnAfterDeserialize()
        {
            m_Target.Clear();

            for (int i = 0; i < m_ListValuePair.Count; i++)
            {
                m_Target.Add(m_ListValuePair[i].Key, m_ListValuePair[i].Value);
            }
        }

        private Dictionary<TKey, TValue> m_Target = null;
        [SerializeField] private List<TempKeyValuePair> m_ListValuePair = null;
    }
}