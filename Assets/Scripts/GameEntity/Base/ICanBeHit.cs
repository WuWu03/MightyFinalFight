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
    void OnHurtMsg(HurtData data);
    void SetCatch(bool value);
}