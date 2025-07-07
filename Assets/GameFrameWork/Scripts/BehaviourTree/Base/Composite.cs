namespace GameFrameWork.BehaviourTree
{
    public abstract class Composite : Task
    {
        public Composite(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args)
        {

        }
    }
}
