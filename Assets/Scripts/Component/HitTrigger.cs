using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TriggerData
{
    public string animName;
    public Vector2[] offestList;
    public Vector2[] sizeList;
}

public class HitTrigger : MonoBehaviour
{
    public TriggerData GetTriggerData(string animName)
    {
        if (TriggerDatas == null) return null;
        for (int i = 0; i < TriggerDatas.Length; i++)
        {
            if (TriggerDatas[i].animName.Equals(animName))
            {
                return TriggerDatas[i];
            }
        }

        return null;
    }

    public TriggerData[] TriggerDatas = null;
}
