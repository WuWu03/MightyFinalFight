using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Serialize
{
    public abstract class BaseScriptableObject<T> : ScriptableObject where T : BaseConfigData
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

    [Serializable]
    public abstract class BaseConfigData : IComparable
    {
        public int id;

        public virtual int CompareTo(object obj)
        {
            BaseConfigData data = obj as BaseConfigData;
            if (data.id < this.id)
                return 1;
            else
                return -1;
        }
    }
}