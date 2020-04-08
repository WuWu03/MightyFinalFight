using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Config
{
    public abstract class BaseScriptableObject<T> : ScriptableObject where T : BaseConfigData
    {
        public T[] Datas = null;
        public T GetData(int id)
        {
            if (Datas == null) return null;
            for (int i = 0; i < Datas.Length; i++)
            {
                if (Datas[i].ID.Equals(id))
                {
                    return Datas[i];
                }
            }

            return null;
        }

        public T Clone()
        {
            return Activator.CreateInstance<T>();
        }
    }

    [Serializable]
    public abstract class BaseConfigData
    {
        public int ID;
    }
}