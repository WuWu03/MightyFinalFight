using System;
using System.Collections.Generic;
using WuWuFramework;
using WuWuFramework.Event;

public class RoleStateParam: WuWuFrameworkEventArg
{
    public bool canAttack { get; set; }
    public bool canBeHit { get; set; }
    public bool canMove { get; set; }
    public bool canJump { get; set; }
    public bool canSkill { get; set; }
    public bool canBeCatch { get; set; }
    public override void Clear()
    {
        this.canAttack = false;
        this.canBeHit = false;
        this.canMove = false;
        this.canJump = false;
        this.canSkill = false;
        this.canBeCatch = false;
    }

    public virtual void CopyTo(RoleStateParam roleStateParam)
    {
        if (roleStateParam == null)
        {
            return;
        }

        roleStateParam.canAttack = this.canAttack;
        roleStateParam.canBeHit = this.canBeHit;
        roleStateParam.canMove = this.canMove;
        roleStateParam.canJump = this.canJump;
        roleStateParam.canSkill = this.canSkill;
        roleStateParam.canBeCatch = this.canBeCatch;
    }
}

public static class FsmStateMap
{
    private static Dictionary<Type, RoleStateParam> s_StateMap = new()
    {
        [typeof(RoleAwaken)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = false,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(RoleDead)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = false,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(RoleDefense)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = true,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(RoleHurt)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = true,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(RoleIdle)] = new RoleStateParam()
        {
            canAttack = true,
            canBeHit = true,
            canJump = true,
            canMove = true,
            canSkill = true,
            canBeCatch = true,
        },
        [typeof(RoleJump)] = new RoleStateParam()
        {
            canAttack = true,
            canBeHit = true,
            canJump = false,
            canMove = true,
            canSkill = true,
            canBeCatch = false,
        },
        [typeof(RoleMove)] = new RoleStateParam()
        {
            canAttack = true,
            canBeHit = true,
            canJump = true,
            canMove = true,
            canSkill = true,
            canBeCatch = true,
        },
        [typeof(RoleSkill)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = true,
            canJump = true,
            canMove = true,
            canSkill = false,
            canBeCatch = true,
        },
        [typeof(RoleSwoon)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = true,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(HeroAttackEnd)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = false,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(HeroCatch)] = new RoleStateParam()
        {
            canAttack = true,
            canBeHit = true,
            canJump = true,
            canMove = false,
            canSkill = true,
            canBeCatch = false,
        },
        [typeof(HeroPickUp)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = false,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
        [typeof(HeroRebirth)] = new RoleStateParam()
        {
            canAttack = false,
            canBeHit = false,
            canJump = false,
            canMove = false,
            canSkill = false,
            canBeCatch = false,
        },
    };

    public static T GetParam<T>(Type stateType) where T : RoleStateParam, new()
    {
        if (s_StateMap.TryGetValue(stateType, out RoleStateParam baseFsmStateParm))
        {
            if (baseFsmStateParm is T result)
            {
                return result;
            }
        }

        return null;
    }
}