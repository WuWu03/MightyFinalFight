using System.Collections.Generic;
using UnityEngine;

public class SkillNoneEffect : SkillBaseEffect
{
    public SkillNoneEffect(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override void Effect(ISkillSelector selector)
    {
    }

    public override void Reset()
    {
    }

    public override void Exit()
    {

    }

    public override void Update(ISkillSelector selector)
    {

    }
}