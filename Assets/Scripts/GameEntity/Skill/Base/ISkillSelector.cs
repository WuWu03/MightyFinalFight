using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISkillSelector
{
    int Index { get; set; }
    List<ICanBeHit> GetTargets(BaseRole owner, SkillData skillData);

    List<GameObject> GetTargetsObj(BaseRole owner, SkillData skillData);
}