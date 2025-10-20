namespace GameFrameWork.Fsm
{
    public interface IFsmMgr
    {
        public int fsmCount { get; }
        public Fsm CreateFsm(object owner, string name);
        public Fsm GetFsm(object owner, string name);
        public bool HasFsm(object owner);
        public void ReleaseFsm(object owner);
        public void ReleaseFsm(Fsm fsm);
    }
}