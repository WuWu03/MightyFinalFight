using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyData : BaseRoleData
{
    public string[] hurtAnims { get; set; }
    public int hpBarWdith { get; set; }
    public bool isBoss { get; set; }

    public override void Clear()
    {
        base.Clear();
        hurtAnims = null;
        hpBarWdith = 0;
        isBoss = false;
    }
}
