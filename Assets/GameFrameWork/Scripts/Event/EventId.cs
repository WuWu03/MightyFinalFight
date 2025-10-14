namespace GameFrameWork.Event
{
    public abstract class EventId
    {
        protected static uint GetEventId()
        {
            s_EventId++;
            return s_EventId;
        }

        private static uint s_EventId = 0;
    }
}