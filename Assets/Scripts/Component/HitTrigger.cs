using UnityEngine;

[System.Serializable]
public class TriggerDatum
{
    public string animName;
    public Vector2[] offestList;
    public Vector2[] sizeList;
}

public class HitTrigger : MonoBehaviour
{
    public TriggerDatum GetTriggerData(string animName)
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

    public TriggerDatum[] triggerData = null;
}
