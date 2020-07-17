using System.Collections.Generic;
using UnityEngine;

public interface ISkillSelector
{
    List<ICanBeHit> GetTargets();

    List<GameObject> GetTargetsObj();

    void Reset();
    void Exit();
}