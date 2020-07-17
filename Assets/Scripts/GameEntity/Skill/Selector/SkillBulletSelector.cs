using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillBulletSelector : SkillBaseSelector
{
    public SkillBulletSelector(SkillData skillData, BaseRole owner, int effectIndex) : base(skillData, owner, effectIndex) { }

    public override List<ICanBeHit> GetTargets()
    {
        return null;
    }

    public override List<GameObject> GetTargetsObj()
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
