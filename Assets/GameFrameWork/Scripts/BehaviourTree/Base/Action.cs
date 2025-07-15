namespace GameFrameWork.BehaviourTree
{
    public abstract class Action : BaseTask
    {
        public Action(string name, int id, object owner, int priority, string args) : base(name, id, owner, priority, args) 
        {

        }
    }
}