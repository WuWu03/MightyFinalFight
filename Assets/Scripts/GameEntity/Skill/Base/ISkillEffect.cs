using System.Collections.Generic;
using UnityEngine;


public interface ISkillEffect
{
    bool IsCompleted { get; }
    void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector);
    void Reset();
}