using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillBulletSelector : ISkillSelector
{
    public int Index { get; set; }

    public List<ICanBeHit> GetTargets(BaseRole owner, SkillData skillData)
    {
        return null;
    }

    public List<GameObject> GetTargetsObj(BaseRole owner, SkillData skillData)
    {
        return null;
    }
}
