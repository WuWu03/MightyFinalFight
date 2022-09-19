using System.Collections.Generic;
using UnityEngine;


public interface ISkillEffect
{
    bool isCompleted { get; }
    void Effect(ISkillSelector selector);
    void Update(ISkillSelector selector);
    void Reset();
    void Exit();
}