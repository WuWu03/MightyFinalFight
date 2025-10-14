public interface ITask
{
    TaskConfigData TaskData { get; }

    public bool isComplete { get; }

    void Enter();

    void Update();

    bool CheckCondition();

    void Trigger();

    bool CanComplete();
}
