namespace GameFrameWork.UI
{
    public enum UILayer : byte
    {
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

    public enum UIDestroyMode : byte
    {
        Always, //UI常驻场景, 此类UI关闭达到一定数量后, 会摧毁最先打开的
        Immediately, //关闭时立即销毁
        Delay, //延迟一段时间销毁
        Eternal, //总是存于场景中, 除非主动销毁
    }

    public abstract class UIBaseSettings
    {
        public abstract string prefabName { get; }

        public abstract float delayDestroyTime { get; }
        
        public virtual bool canPopUp { get; }//是否可以回弹
        
        public abstract UILayer layer { get; }

        public abstract UIDestroyMode destroyMode { get; }
    }
}