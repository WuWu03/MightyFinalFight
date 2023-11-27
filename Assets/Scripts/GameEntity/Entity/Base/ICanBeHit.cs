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

    EntityAttribute entityAttribute
    {
        get;
    }

    bool IsHurtWillDie(int attackValue);
    void OnHurtMsg(HurtData data);
    void SetCatch(bool value);
    void SetThrow(bool value);
}