using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public interface IExporter
    {
        void Export(UIRef[] uiRefs, UIRefSetting setting);

        string CopyRef(UIRef[] uiRefs);
    }
}
