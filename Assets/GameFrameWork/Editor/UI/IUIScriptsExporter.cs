using GameFrameWork.UI;

namespace GameFrameWork.Editor
{
    public interface IUIScriptsExporter
    {
        void Export(UIRef[] uiRefs, UIRefSetting setting);

        string CopyRef(UIRef[] uiRefs);
    }
}
