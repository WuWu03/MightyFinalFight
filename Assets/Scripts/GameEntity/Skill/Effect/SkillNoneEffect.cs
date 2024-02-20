using System.Collections.Generic;
using UnityEngine;

public class SkillNoneEffect : SkillBaseEffect
{
    public SkillNoneEffect(SkillBaseDeployer deployer, SkillConfigData skillData, BaseRole owner, int effectIndex) : base(deployer, skillData, owner, effectIndex) 
    { 

    }

    public override void Effect(ISkillSelector selector)
    {
        Complete();
    }

}