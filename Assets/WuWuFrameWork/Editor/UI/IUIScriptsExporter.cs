using WuWuFramework.UI;

namespace WuWuFramework.Editor
{
    public interface IUIScriptsExporter
    {
        void Export(UIRef[] uiRefs, UIRefSetting setting);

        string CopyRef(UIRef[] uiRefs);
    }
}
