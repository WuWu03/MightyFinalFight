using System;
using System.Collections.Generic;
using UnityEngine;

namespace WuWuFramework.Serialize
{
    [Serializable]
    public class SerializableList<T> : List<T>,ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> m_Data = new();
        
        public void OnBeforeSerialize()
        {
            m_Data.Clear();

            foreach (var data in this)
            {
                m_Data.Add(data);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            
            foreach (var data in m_Data)
            {
                this.Add(data);
            }
        }
    }
}