using GameFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResPath : GameFrameWork.UI.UIResPath
{
    public override string GetUIResPath(string name)
    {
        return PathUtil.FormatPath(ResDefine.UI_PATH, name);
    }

    public override string GetUISpritePath(string name)
    {
        return PathUtil.FormatPath(ResDefine.UISPRITE_PATH, name);
    }
}
