namespace Runtime
{
    public class BaseRoleInfo : ObjectInfo
    {
        public string Name { get; set; }
        public string ResName { get; set; }
        public int ATK { get; set; }
        public float MoveSpeed { get; set; }
        public int[] Skills { get; set; }
    }
}
