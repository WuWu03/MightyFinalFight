namespace GameFrameWork.UI
{
    public abstract class ScrollLayoutGroupViewItem : LayoutGroupViewItem
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

        public override void ReleaseItem()
        {
            base.ReleaseItem();
            dataIndex = 0;
        }
    }
}