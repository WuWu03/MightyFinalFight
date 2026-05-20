namespace GameFrameWork.Event
{
    public interface IEventSender
    {
        public void Dispatch(EventPool eventPool);
    }
}