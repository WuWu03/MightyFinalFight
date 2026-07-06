namespace WuWuFramework.UI
{
    public abstract class UIBaseViewSettings : IUIViewSettings
    {
        public abstract string prefabName { get; }

        public abstract float delayDestroyTime { get; }

        public virtual bool canPopUp { get { return false; } }//是否可以回弹

        public abstract UILayer layer { get; }

        public abstract UIDestroyMode destroyMode { get; }
    }
}