using FrameWork.GameEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillMoveTargetEffect : ISkillEffect
{
    public bool IsCompleted
    {
        get
        {
            return m_IsCompleted;
        }
    }


    public int Index
    {
        get;
        set;
    }

    public void Effect(BaseRole owner, SkillData skillData, ISkillSelector selector)
    {
        List<GameObject> targets = selector.GetTargetsObj(owner, skillData);
        if (targets.Count < 1)
        {
            m_IsCompleted = true;
            return;
        }

        BaseObject bo = targets[0].GetComponent<BaseObject>();
        float targetY = bo.Pos.y;
        bo.SetPos2(owner.Pos.x + skillData.SkillEffects[Index].MoveTarget.x * owner.Dir,
                   owner.Pos.y + skillData.SkillEffects[Index].MoveTarget.y);
        bo.UpdatePos2(bo.Pos.x, targetY);
        m_IsCompleted = true;
    }


    public void Reset()
    {
        m_IsCompleted = false;
    }

    private bool m_IsCompleted = false;
}