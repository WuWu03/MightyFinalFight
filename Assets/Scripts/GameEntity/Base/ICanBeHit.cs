using FrameWork.GameEntity;
using UnityEngine;

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

    Vector2 HurtPos
    {
        get;
    }
    void OnHurtMsg(HurtData data);
    void SetCatch(bool value);
}