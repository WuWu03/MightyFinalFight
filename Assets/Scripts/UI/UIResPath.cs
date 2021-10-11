using GameFrameWork.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResPath : GameFrameWork.UI.UIResPath
{
    public override string GetUIResPath(string name)
    {
        return PathUtil.FormatPath(ResDefine.UIPath, name);
    }

    public override string GetUISpritePath(string name)
    {
        return PathUtil.FormatPath(ResDefine.UISpritePath, name);
    }
}
