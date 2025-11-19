namespace GameFrameWork.UI
{
    public abstract class ScrollListItem : BaseListItem
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

        protected override void OnReleaseItem()
        {
            dataIndex = 0;
        }
    }
}