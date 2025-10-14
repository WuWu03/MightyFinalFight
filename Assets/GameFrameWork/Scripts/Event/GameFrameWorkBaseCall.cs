namespace GameFrameWork.Event
{
    public abstract class GameFrameWorkBaseCall<A> : GameFrameWorkEventArg
    {
        public A action { get; set; }
        public abstract void Invoke();

        public override void Clear()
        {
            action = default;
        }
    }
}