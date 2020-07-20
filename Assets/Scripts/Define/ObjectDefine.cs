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

public enum ObjectMsgType
{
    NONE = 0,
    Idle = 1,
    Move = 2,
    Attack = 3,
    Jump = 4,
    Hurt = 5,
}

public struct SoundName
{
    public const string DefaultHurt = "OnHit02";
}