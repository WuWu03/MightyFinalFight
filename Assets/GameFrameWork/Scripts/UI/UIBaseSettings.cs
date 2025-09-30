namespace GameFrameWork.UI
{
    public enum UILayer : byte
    {
        Panel,
        Scene,
        Bg,
        MainWindow,
        Window1,
        Window2,
        Tips,
        Guide,
        Message,
        Mask,
        Load,
    }

    public enum UICloseMode : byte
    {
        Always,// UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先关闭的
        Destroy,// 关闭时立即销毁
        DelayDestroy,// 延迟一段时间销毁
        Eternal,// 总是存于场景中, 除非主动销毁
    }

    public abstract class UIBaseSettings
    {
        public abstract string name { get; }

        public abstract float unLoadTime { get; }
        
        public bool canPopUp { get; }//是否可以回弹
        
        public abstract UILayer Layer { get; }

        public abstract UICloseMode CloseMode { get; }
    }
}