using System;
using System.Collections.Generic;

public abstract class BaseFsmStateParm
{
}

public class RoleStateParam : BaseFsmStateParm
{
    public bool canAttack { get; set; }
    public bool canBeHit { get; set; }
    public bool canMove { get; set; }
    public bool canJump { get; set; }
    public bool canSkill { get; set; }
    public bool canBeCatch { get; set; }
}

public static class FsmStateMap
{
    private static Dictionary<Type, BaseFsmStateParm> s_StateMap = new()
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
        [typeof(HeroPickUp)] = new RoleStateParam()
        {
            canAttack = true,
            canBeHit = false,
            canJump = true,
            canMove = true,
            canSkill = true,
            canBeCatch = false,
        },
    };

    public static T GetParam<T>(Type stateType) where T : BaseFsmStateParm, new()
    {
        if (s_StateMap.TryGetValue(stateType, out BaseFsmStateParm baseFsmStateParm))
        {
            if (baseFsmStateParm is T result)
            {
                return result;
            }
        }

        return null;
    }
}