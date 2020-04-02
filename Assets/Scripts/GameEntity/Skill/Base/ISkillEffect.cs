using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public interface ISkillEffect
    {
        bool IsCompleted { get; }
        void Effect(BaseAvatar owner, SkillData skillData, ISkillSelector selector);
    }
}
