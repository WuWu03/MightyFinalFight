public enum ObjectType
{
    NONE = 0,
    Player = 1,//玩家
    Enemy = 2,//敌人
    NPC = 3,//npc
    Barrel = 4,//油桶
    Weapon = 5,//武器
    Consume = 6,//消耗品
    BreakItem = 7,//可破坏的物体
    CantBreakItem = 8,//不可破坏物体
}

public struct SoundName
{
    public const string DefaultHurt = "Sound/OnHit02";
    public const string DefaultDrop = "Sound/OnDrop";
    public const string DefaultJump = "Sound/Jump";
}

public struct LayerName
{
    public const string Map = "Map";
    public const string Unit = "Unit";
}

public struct ConstField
{
    public const float CatchTime = 2f;
    public const float CatchAttackTime = 0.3f;
    public const float EnemyHPBarHideTime = 4f;
    public const float CollectTime = 15f;
}