using System.Collections.Generic;

public interface ISkillSelector
{
    List<ICanBeHit> GetTargets();

    void Reset();
    void Exit();
}