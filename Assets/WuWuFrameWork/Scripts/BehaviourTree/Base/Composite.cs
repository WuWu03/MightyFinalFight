namespace WuWuFramework.BehaviourTree
{
    public abstract class Composite : Task
    {
        public Composite(int id, object owner, int priority, string args) : base(id, owner, priority, args)
        {

        }
    }
}
