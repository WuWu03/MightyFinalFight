public interface ITaskTrigger 
{
    bool isComplete { get; }
    void Enter();
    void Trigger();
    void Complete();
}