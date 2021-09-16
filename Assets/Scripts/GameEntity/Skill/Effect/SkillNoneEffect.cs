using System.Collections.Generic;
using UnityEngine;

public class SkillNoneEffect : SkillBaseEffect
{
    public SkillNoneEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) 
    { 

    }

    public override void Effect(ISkillSelector selector)
    {
        Complete();
    }

}