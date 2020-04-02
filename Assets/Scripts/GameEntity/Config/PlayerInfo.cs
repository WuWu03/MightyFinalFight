using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class PlayerInfo:BaseRoleInfo
    {
        public string Desc { get; set; }
        public string HeadIcon { get; set; }
        public int CurrLevel { get; set; }
        public int CurrExp { get; set; }
        public Vector2 JumpForce { get; set; }//跳跃力
        public List<AttackConfig> JumpAttackConfig { get; set; }//跳跃攻击1配置
        public float[] AttackWait { get; set; }//攻击连击时间
    }
}
