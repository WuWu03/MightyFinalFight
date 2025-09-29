using System.Collections.Generic;

public class SkillNoneSelector : SkillBaseSelector
{
    public SkillNoneSelector(SkillConfigData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override List<ICanBeHit> GetTargets()
    {
        return null;
    }

    public override void Reset()
    {

    }

    public override void Exit()
    {

    }
}
