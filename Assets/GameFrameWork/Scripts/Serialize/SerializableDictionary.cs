using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Serialize
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new();

        [SerializeField]
        private List<TValue> values = new();

        // 序列化回调，用于在序列化时填充keys和values
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // 反序列化回调，用于在反序列化时重建字典
        public void OnAfterDeserialize()
        {
            this.Clear();

            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            {
                this[keys[i]] = values[i];
            }
        }
    }
}