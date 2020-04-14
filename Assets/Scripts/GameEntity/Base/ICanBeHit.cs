public interface ICanBeHit
{
    bool CanBeHit
    {
        get;
    }
    void OnHurtMsg(HurtData data);
}