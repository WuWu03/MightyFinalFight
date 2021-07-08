using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHeroSkillData : BaseRoleSkillData
{
    public int CatchAttackID { get; set; }
    public int ThrowAttackID { get; set; }
    public int WeaponAttackID { get; set; }
    public int ThrowWeaponID { get; set; }

    public override void Clear()
    {
        base.Clear();
        CatchAttackID = 0;
        ThrowAttackID = 0;
        WeaponAttackID = 0;
        ThrowWeaponID = 0;
    }
}
