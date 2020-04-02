using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public interface ISkillSelector
    {
        List<GameObject> GetTargets(BaseRole owner, SkillData skillData);
    }
}
