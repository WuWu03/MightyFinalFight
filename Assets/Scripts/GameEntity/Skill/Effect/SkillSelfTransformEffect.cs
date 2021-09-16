using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelfTransformEffect : SkillBaseEffect
{
    public SkillSelfTransformEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
    }

}
