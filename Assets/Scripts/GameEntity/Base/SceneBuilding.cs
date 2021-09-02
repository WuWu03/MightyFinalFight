using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBuilding : BaseSceneObject
{
    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        SetLayer(LayerName.Map);
    }
}
