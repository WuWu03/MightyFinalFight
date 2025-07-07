namespace GameFrameWork.BehaviourTree
{
    public class Decorator : Task
    {
        public Decorator(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args) { }
    }
}