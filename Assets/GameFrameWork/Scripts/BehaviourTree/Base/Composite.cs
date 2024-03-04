namespace GameFrameWork.BehaviourTree
{
    public abstract class Composite : Task
    {
        public Composite(string name, string args, object owner, int priority) : base(name, args, owner, priority) { }
    }
}
