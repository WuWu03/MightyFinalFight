public interface ICanBeHit
{
    bool CanBeHit
    {
        get;
    }

    bool IsBeCatch
    {
        get;
    }

    bool IsBeThrow
    {
        get;
    }

    bool IsDead
    {
        get;
    }

    void OnHurtMsg(HurtData data);
    void SetCatch(bool value);
    void SetThrow(bool value);
}