using UnityEngine;
using FrameWork.GameEntity;

namespace Runtime
{
    public class HurtData:BaseData
    {
        public HurtData() : base("HurtData") { }

        public HurtData(string sender, string receiver) : base("HurtData", sender, receiver) { }
        public Vector2 AttackForce { get; set; }
        public int AttackValue { get; set; }
        public int AttackerID {get;set;}
        public bool IsSwoon { get; set; }//是否击飞
        public float AttackerDir { get; set; }
    }
}
