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

    bool isSwoon
    {
        get;
    }

    EntityAttribute entityAttribute
    {
        get;
    }

    bool IsHurtWillDie(int attackValue);
    void OnHurtMsg(HurtStateArg arg);
    void SetIsBeCatch(bool value);
    void SetIsBeThrow(bool value);
}