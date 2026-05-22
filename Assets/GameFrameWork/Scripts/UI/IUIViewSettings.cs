namespace GameFrameWork.UI
{
    public interface IUIViewSettings
    {
        public string prefabName { get; }

        public float delayDestroyTime { get; }

        public bool canPopUp { get; }//是否可以回弹

        public UILayer layer { get; }

        public UIDestroyMode destroyMode { get; }
    }
}
