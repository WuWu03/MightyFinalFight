using WuWuFramework;

public class BaseHeroSkillData : BaseRoleSkillData
{
    public int catchAttackID { get; set; }
    public int throwAttackID { get; set; }
    public int weaponAttackID { get; set; }
    public int throwWeaponID { get; set; }

    public static BaseHeroSkillData Create()
    {
        return ReferencePool.Acquire<BaseHeroSkillData>();
    }

    public override void Clear()
    {
        base.Clear();
        catchAttackID = 0;
        throwAttackID = 0;
        weaponAttackID = 0;
        throwWeaponID = 0;
    }
}
