using UnityEngine;

namespace Runtime
{
    public class JumpData:BaseData
    {
        public JumpData():base("JumpData"){}

        public JumpData(string sender, string receiver) : base("JumpData", sender, receiver) { }
        
        public Vector2 Dir 
        {
            get; 
            set; 
        }

        public bool IsToAttack { get; set; }
    }
}
