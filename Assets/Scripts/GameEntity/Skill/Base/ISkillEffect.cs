using System.Collections.Generic;
using UnityEngine;


public interface ISkillEffect
{
    bool IsCompleted { get; }
    void Effect(ISkillSelector selector);
    void Update();
    void Reset();
    void Exit();
}