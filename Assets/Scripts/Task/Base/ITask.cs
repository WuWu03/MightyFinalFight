public interface ITask
{
    TaskConfigData taskData { get; }

    public bool isComplete { get; }

    void Enter();

    void Update();

    bool CheckCondition();

    void Trigger();

    bool CanComplete();
}
