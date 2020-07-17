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

    bool IsDead
    {
        get;
    }

    void OnHurtMsg(HurtData data);
    void SetCatch(bool value);
}