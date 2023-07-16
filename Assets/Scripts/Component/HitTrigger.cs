using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerData
{
    public string AnimName;
    public Vector2 Offest;
    public Vector2 Size;
}

public class HitTrigger : MonoBehaviour
{
    public TriggerData GetTriggerData(string animName)
    {
        if (TriggerDatas == null) return null;
        for (int i = 0; i < TriggerDatas.Length; i++)
        {
            if (TriggerDatas[i].AnimName.Equals(animName))
            {
                return TriggerDatas[i];
            }
        }

        return null;
    }

    public TriggerData[] TriggerDatas = null;
}
