using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class DropTragData : BaseData
    {
        public DropTragData() : base("HurtData") { }
        public DropTragData(string sender, string receiver) : base("HurtData", sender, receiver) { }
        public Vector2 InitPos;
        public int AttackValue;//伤害值
    }
}