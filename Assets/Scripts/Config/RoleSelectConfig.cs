using GameFrameWork.Serialize;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleSelectConfig : BaseScriptableObject<RoleSelectConfigData>
{

}

[Serializable]
public class RoleSelectConfigData: BaseConfigData
{
    public int CharacterId;
}
