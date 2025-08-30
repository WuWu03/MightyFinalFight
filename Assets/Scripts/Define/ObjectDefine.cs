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

public class SoundName
{
    public const string DefaultHurt = "Sound/OnHit02.ogg";
    public const string DefaultDrop = "Sound/OnDrop.ogg";
    public const string DefaultJump = "Sound/Jump.ogg";
    public const string Hurt = "Sound/OnHit.ogg";
    public const string Barrel = "Sound/Barrel.ogg";
    public const string Eat = "Sound/OnEat.ogg";
    public const string GetRobot = "Sound/GetRobot.ogg";
    public const string FallDown = "Sound/OnFallDown.ogg";
    public const string LevelUp = "Sound/LevelUp.ogg";
    public const string Break = "Sound/Break.ogg";
    public const string FallDownHigh = "Sound/FallDownHigh.ogg";
    public const string OnSelect = "Sound/OnSelect.ogg";
    public const string OnSelected = "Sound/OnSelected.ogg";
    public const string BicycleKick = "Sound/BicycleKick.ogg";
    public const string Bonus = "Sound/Bonus.ogg";
    public const string OnBlow = "Sound/OnBlow.ogg";
    public const string OnHit02 = "Sound/OnHit02.ogg";
    public const string BgmOpening = "BGM/bgm08Opening.ogg";
    public const string BgmTitle = "BGM/bgm09Title.ogg";
    public const string BgmCharacter_Start = "BGM/bgm10Character_Start.ogg";
    public const string BgmCharacter_Loop = "BGM/bgm10Character_Loop.ogg";
    public const string BgmClear = "BGM/bgm11Clear.wav";
}

public class LayerName
{
    public const string UI = "UI";
    public const string Map = "Map";
    public const string Unit = "Unit";
    public const string Bullet = "Bullet";
}

public class CameraName
{
    public const string MainCamera = "MainCamera";
    public const string RoleCamera = "RoleCamera";
}

public class CameraTag
{
    public const string MainCamera = "MainCamera";
    public const string Untagged = "Untagged";
}

public class CameraDepth
{
    public const int MainCamera = 0;
    public const int RoleCamera = 1;
}

public class ConstField
{
    public const float CatchTime = 2f;
    public const float CatchAttackTime = 0.3f;
    public const float EnemyHPBarHideTime = 4f;
    public const float CollectTime = 0f;
}