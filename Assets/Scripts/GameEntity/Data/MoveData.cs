using UnityEngine;

namespace Runtime
{
    public class MoveData : BaseData
    {
        public Vector2 Dir { get; set; }
        public MoveData() : base("MoveData") { }

        public MoveData(string sender, string receiver) : base("MoveData", sender, receiver) { }
    }
}
