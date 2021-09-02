using GameFrameWork.Serialize;
using System;
using UnityEngine;

public class TalkConfig : BaseScriptableObject<TalkConfigData>
{
}

[Serializable]
public class TalkConfigData : BaseConfigData
{
    [SerializeField]
    public string Text;
}
