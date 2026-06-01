using System;
using System.Collections.Generic;
using UnityEngine;

namespace WuWuFramework.Serialize
{
    public abstract class BaseScriptableObject<T> : ScriptableObject where T : BaseScriptableConfigData
    {
        public BaseScriptableObject()
        {
            listDatas = new List<T>();
        }

        public List<T> listDatas = null;
        public virtual T GetData(int id)
        {
            if (listDatas == null)
            {
                return null;
            }

            for (int i = 0; i < listDatas.Count; i++)
            {
                if (listDatas[i].id.Equals(id))
                {
                    return listDatas[i];
                }
            }

            return null;
        }

        public virtual T GetDataByIndex(int index)
        {
            if (listDatas == null)
            {
                return null;
            }

            if(index < 0 || index >= listDatas.Count)
            {
                return null;
            }

            return listDatas[index];
        }

        public virtual void AddData(T data)
        {
            if(listDatas == null)
            {
                listDatas = new List<T>();
            }

            listDatas.Add(data);
        }

        public T Clone()
        {
            return Activator.CreateInstance<T>();
        }
    }
}