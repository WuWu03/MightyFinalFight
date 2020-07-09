public enum ObjectType
{
    NONE = 0,
    Player = 1,//玩家
    Monster = 2,//怪物
    NPC = 3,//npc
    SceneItem = 4,//不可破坏的场景物体 箱子,掉落道具等
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

public enum KeyCodeType
{
    Up = 1,
    Down = 2,
    Left = 3,
    Rigth = 4,
    Attack = 5,
    Jump = 6,
}