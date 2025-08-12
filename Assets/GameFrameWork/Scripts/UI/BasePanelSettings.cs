namespace GameFrameWork.UI
{
    public enum PanelType
    {
        Root,//根界面（主界面）
        Normal,//一般界面
        Pop,//弹出界面
    }

    public enum PanelLayer
    {
        Layer1,
        Layer2,
        Layer3,
        Layer4,
        Layer5,
        Layer6,
        Layer7,
        Layer8,
    }

    public enum PanelCloseMode
    {
        Always = 1,         // UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先关闭的
        Destroy = 2,        // 关闭时立即销毁
        DelayDestroy = 3,   // 延迟一段时间销毁
        Eternal = 4,        // 总是存于场景中, 除非主动销毁
    }

    public abstract class BasePanelSettings
    {
        public abstract string panelName { get; }

        public abstract float panelUnLoadTime { get; }

        public abstract PanelType panelType { get; }

        public abstract PanelLayer panelLayer { get; }

        public abstract PanelCloseMode panelCloseMode { get; }
    }
}
