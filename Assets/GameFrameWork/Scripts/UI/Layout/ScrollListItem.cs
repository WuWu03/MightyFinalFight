namespace GameFrameWork.UI
{
    public abstract class ScrollListItem : LayoutGroupViewItem
    {
        public override int id
        {
            get
            {
                return dataIndex + 1;
            }
        }

        public int dataIndex
        {
            get;
            set;
        }

        public abstract void OnUpdate();

        public override void ReleaseItem()
        {
            base.ReleaseItem();
            dataIndex = 0;
        }
    }
}