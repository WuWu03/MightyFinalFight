namespace GameFrameWork.UI
{
    public abstract class UIBaseView
    {
        public void InitView(UIRefRoot root)
        {
            OnInitView(root);
        }

        protected abstract void OnInitView(UIRefRoot root);
    }
}