public enum ObjectType
{
    NONE = 0,
    Player = 1,//玩家
    Monster = 2,//怪物
    NPC = 3,//npc
    Weapon = 4,
    Consume = 5,//掉落物
    BreakItem = 5,//可破坏的物体
    CantBreakItem = 6,//不可破坏物体
}

public struct BehaviourType
{
    public const int Idle = 0;
    public const int RandomPos = 1;
    public const int RoundPos = 2;
    public const int Attack = 3;
    public const int Skill = 4;
}

public struct SoundName
{
    public const string DefaultHurt = "Sound/OnHit02";
    public const string DefaultDrop = "Sound/OnDrop";
    public const string DefaultJump = "Sound/Jump";
}