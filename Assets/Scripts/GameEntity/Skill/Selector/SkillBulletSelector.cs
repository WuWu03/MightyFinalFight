using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime
{
    public class SkillBulletSelector : ISkillSelector
    {
        public List<GameObject> GetTargets(BaseAvatar owner, SkillData skillData)
        {
            return null;
        }
    }
}
