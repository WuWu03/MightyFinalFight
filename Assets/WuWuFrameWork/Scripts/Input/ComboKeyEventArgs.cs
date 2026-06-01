using WuWuFramework.Event;

namespace WuWuFramework.Input
{
    public class ComboKeyEventArgs : WuWuFrameworkEventArg
    {
        public KeyType[] keys { get; set; }
        public int eventId { get; set; }
        public WuWuFrameworkAction<int, bool> keyEvent { get; set; }

        public static ComboKeyEventArgs Create(KeyType[] keys, int eventId, WuWuFrameworkAction<int, bool> keyEvent)
        {
            ComboKeyEventArgs args = ReferencePool.Acquire<ComboKeyEventArgs>();
            args.keys = keys;
            args.eventId = eventId;
            args.keyEvent = keyEvent;
            return args;
        }
        public override void Clear()
        {
            keys = null;
            keyEvent = null;
        }
    }
}