namespace GameFrameWork.UI
{
    public abstract class BasePanelComponent
    {
        public void InitComponent(UIRefRoot root)
        {
            OnInitComponent(root);
        }

        protected abstract void OnInitComponent(UIRefRoot root);
    }
}