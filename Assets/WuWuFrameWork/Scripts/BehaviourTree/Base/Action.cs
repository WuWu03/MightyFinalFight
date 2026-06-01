namespace WuWuFramework.BehaviourTree
{
    public abstract class Action : BaseTask
    {
        public Action(int id, object owner, int priority, string args) : base(id, owner, priority, args) 
        {

        }
    }
}