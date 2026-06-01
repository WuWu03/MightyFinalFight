namespace WuWuFramework.UI
{
    public interface IUIViewPresenter
    {
        public void SetView(IUIView view);
        public void Open(object arg);
        public void Update();
        public void Close();
        public void Show(object arg);
        public void Hide();
        public void Destroy();
    }
}