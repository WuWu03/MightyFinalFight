using System.Collections.Generic;
using UnityEngine;


public interface ISkillEffect
{
    bool IsCompleted { get; }
    int Index { get; set; }
    void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector);
    void Reset();
}