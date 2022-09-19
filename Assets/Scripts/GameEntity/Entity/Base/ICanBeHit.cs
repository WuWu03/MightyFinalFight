public interface ICanBeHit
{
    bool canBeHit
    {
        get;
    }

    bool isBeCatch
    {
        get;
    }

    bool isBeThrow
    {
        get;
    }

    bool isDead
    {
        get;
    }

    void OnHurtMsg(HurtData data);
    void SetCatch(bool value);
    void SetThrow(bool value);
}