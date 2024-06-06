namespace GameFrameWork.UI
{
    public abstract class BasePanelSettings
    {
        public abstract string panelName { get; }

        public abstract float panelUnLoadTime { get; }

        public abstract UIMgr.Type panelType { get; }

        public abstract UIMgr.Layer panelLayer { get; }

        public abstract UIMgr.CloseMode panelCloseMode { get; }
    }
}
