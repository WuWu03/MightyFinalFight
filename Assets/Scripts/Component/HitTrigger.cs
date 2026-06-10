using UnityEngine;

[System.Serializable]
public class TriggerData
{
    public string animName;
    public bool useAttackBox;
    public Vector2[] attackBoxOffsets;
    public Vector2[] attackBoxSizes;
    public Vector2[] defendBoxOffsets;
    public Vector2[] defendBoxSizes;
}

public class HitTrigger : MonoBehaviour
{
    public TriggerData GetTriggerData(string animName)
    {
        if (triggerData == null)
        {
            return null;
        }

        foreach (var triggerDatum in triggerData)
        {
            if (triggerDatum.animName.Equals(animName))
            {
                return triggerDatum;
            }
        }

        return null;
    }

    public TriggerData[] triggerData = null;
}