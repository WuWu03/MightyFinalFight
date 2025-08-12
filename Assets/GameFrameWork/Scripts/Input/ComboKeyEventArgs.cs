namespace GameFrameWork.Input
{
    public class ComboKeyEventArgs : BaseEventArgs
    {
        public KeyType[] keys { get; set; }
        public int eventId { get; set; }
        public GameFrameWorkAction<int, bool> keyEvent { get; set; }

        public static ComboKeyEventArgs Create(KeyType[] keys, int eventId, GameFrameWorkAction<int, bool> keyEvent)
        {
            ComboKeyEventArgs args = ReferencePool.Acquire<ComboKeyEventArgs>();
            args.keys = keys;
            args.eventId = eventId;
            args.keyEvent = keyEvent;
            return args;
        }
        public override void Clear()
        {
            base.Clear();
            keys = null;
            keyEvent = null;
        }
    }
}