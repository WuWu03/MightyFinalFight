namespace GameFrameWork.UI
{
    public abstract class BasePanelComponent
    {
        public BasePanelComponent(UIRefRoot root)
        {
            InitComponent(root);
        }

        protected abstract void InitComponent(UIRefRoot root);
    }
}
