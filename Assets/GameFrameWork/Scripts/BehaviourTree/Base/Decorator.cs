namespace GameFrameWork.BehaviourTree
{
    public class Decorator : Task
    {
        public Decorator(int id, object owner, int priority, string args) : base(id, owner, priority, args) { }
    }
}