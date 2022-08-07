using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoleData : BaseSceneObjectData
{
    public bool CatchControl { get; set; }

    public override void Clear()
    {
        base.Clear();
        CatchControl = false;
    }
}
