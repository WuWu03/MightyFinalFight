namespace GameFrameWork
{
    public abstract class BaseEventArgs : GameFrameWorkEventArgs
    {
        public int id { get; set; }

        public override void Clear()
        {
            id = 0;
        }
    }
}