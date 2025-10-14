public sealed class EventId : GameFrameWork.Event.EventId
{
    public static readonly uint TalkEndEvent = GetEventId();
    public static readonly uint StageEnterStartEvent = GetEventId();
    public static readonly uint StageEnterEndEvent = GetEventId();
}