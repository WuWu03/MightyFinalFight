namespace Runtime
{
    public class IdleData : BaseData
    {
        public IdleData() : base("IdleData") { }
        public IdleData(string sender, string receiver) : base("IdleData", sender, receiver) { }
    }
}
