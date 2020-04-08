using Runtime.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public interface ISkillEffect
    {
        bool IsCompleted { get; }
        void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector);
        void Reset();
    }
}
