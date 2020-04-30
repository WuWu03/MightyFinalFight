using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillBulletSelector : ISkillSelector
{
    public List<ICanBeHit> GetTargets(BaseRole owner, SkillData skillData)
    {
        return null;
    }
}
