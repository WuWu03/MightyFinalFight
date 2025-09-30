namespace GameFrameWork.UI
{
    public abstract class UIBaseComponent
    {
        public void InitComponent(UIRefRoot root)
        {
            OnInitComponent(root);
        }

        protected abstract void OnInitComponent(UIRefRoot root);
    }
}