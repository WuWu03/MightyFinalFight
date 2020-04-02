namespace Runtime
{
    public enum AttackType
    {
        Attack,
        JumpAttack,
    }
    public class AttackData:BaseData
    {
        public AttackData():base("AttackData") {}

        public AttackData(string sender, string receiver) : base("AttackData", sender, receiver) { }

        public string AnimationName { get; set; }
        public float Dir { get; set; }
        public bool CanChangeDir { get; set; }
        public AttackType AttackType { get; set; }
    }
}
