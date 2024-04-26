
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SerializableList<T>
{
    public T this[int index]
    {
        get
        {
            return target[index];
        }
        set
        {
            target[index] = value; 
        }
    }

    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public SerializableList()
    {
        target = new List<T>();
    }

    public void Add(T item)
    {
        target.Add(item);
    }

    public void AddRange(T[] values)
    {
        target.AddRange(values);
    }

    public void Remove(T item)
    {
        target.Remove(item);
    }

    public void RemoveAt(int index)
    {
        target.RemoveAt(index);
    }

    public bool Contains(T item)
    {
        return target.Contains(item);
    }
}