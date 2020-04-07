namespace Runtime
{
    public class AttackData:BaseData
    {
        public AttackData():base("AttackData") {}

        public AttackData(string sender, string receiver) : base("AttackData", sender, receiver) { }

        public string AnimationName { get; set; }
        public float Dir { get; set; }
        public bool CanChangeDir { get; set; }
    }
}
