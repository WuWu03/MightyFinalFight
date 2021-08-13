using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyData :BaseRoleData
{
    public string[] HurtAnim { get; set; }
    public int HpBarWdith { get; set; }
    public override void Clear()
    {
        base.Clear();
        HurtAnim = null;
        HpBarWdith = 0;
    }
}
