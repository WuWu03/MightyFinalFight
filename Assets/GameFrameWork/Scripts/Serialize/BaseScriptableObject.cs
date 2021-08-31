using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Serialize
{
    public abstract class BaseScriptableObject<T> : ScriptableObject where T : BaseConfigData
    {
        public List<T> Datas = null;
        public virtual T GetData(int id)
        {
            if (Datas == null)
            {
                return null;
            }

            for (int i = 0; i < Datas.Count; i++)
            {
                if (Datas[i].Id.Equals(id))
                {
                    return Datas[i];
                }
            }

            return null;
        }

        public virtual T GetDataByIndex(int index)
        {
            if (Datas == null)
            {
                return null;
            }

            if(index < 0 || index >= Datas.Count)
            {
                return null;
            }

            return Datas[index];
        }

        public virtual void AddData(T data)
        {
            if(Datas == null)
            {
                Datas = new List<T>();
            }

            Datas.Add(data);
        }

        public T Clone()
        {
            return Activator.CreateInstance<T>();
        }
    }

    [Serializable]
    public abstract class BaseConfigData : IComparable
    {
        public int Id;

        public int CompareTo(object obj)
        {
            BaseConfigData data = obj as BaseConfigData;
            if (data.Id < this.Id)
                return 1;
            else
                return -1;
        }
    }
}