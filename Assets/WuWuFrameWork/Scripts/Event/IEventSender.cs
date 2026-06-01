namespace WuWuFramework.Event
{
    public interface IEventSender
    {
        public void Dispatch(EventPool eventPool);
    }
}