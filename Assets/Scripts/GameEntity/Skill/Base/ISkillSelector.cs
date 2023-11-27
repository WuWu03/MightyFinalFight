using System.Collections.Generic;
using UnityEngine;

public interface ISkillSelector
{
    List<ICanBeHit> GetTargets();

    void Reset();
    void Exit();
}