
//===================================================
//作者：WuWu                                          
//创建时间：2024-06-06 11:09:24
//备注：此代码为工具生成 请勿手工修改
//===================================================
using GameFrameWork;
using GameFrameWork.ConfigData;
using UnityEngine;

/// <summary>
/// Level.xlsx数据表
/// SheetName:Sheet1
/// </summary>
public class LevelConfigData : BaseConfigData
{
	/// <summary>
	/// 角色id
	/// </summary>
	public int roleId { get; private set; }

	/// <summary>
	/// 等级
	/// </summary>
	public int level { get; private set; }

	/// <summary>
	/// 生命
	/// </summary>
	public int hpValue { get; private set; }

	/// <summary>
	/// 攻击力
	/// </summary>
	public int attackValue { get; private set; }

	/// <summary>
	/// 防御力
	/// </summary>
	public int defenseValue { get; private set; }

	/// <summary>
	/// 升级经验
	/// </summary>
	public int exp { get; private set; }

	/// <summary>
	/// 攻速
	/// </summary>
	public float attackSpeed { get; private set; }

	/// <summary>
	/// 暴击率
	/// </summary>
	public int criticalValue { get; private set; }

	/// <summary>
	/// 移动速度
	/// </summary>
	public float moveSpeed { get; private set; }

	/// <summary>
	/// 跳跃力
	/// </summary>
	public Vector2 jumpForce { get; private set; }

	/// <summary>
	/// 血条长度
	/// </summary>
	public float hpBarWidth { get; private set; }

	public LevelConfigData Clone()
	{
		LevelConfigData levelConfigData = new LevelConfigData();
		levelConfigData.roleId = this.roleId;
		levelConfigData.level = this.level;
		levelConfigData.hpValue = this.hpValue;
		levelConfigData.attackValue = this.attackValue;
		levelConfigData.defenseValue = this.defenseValue;
		levelConfigData.exp = this.exp;
		levelConfigData.attackSpeed = this.attackSpeed;
		levelConfigData.criticalValue = this.criticalValue;
		levelConfigData.moveSpeed = this.moveSpeed;
		levelConfigData.jumpForce = this.jumpForce;
		levelConfigData.hpBarWidth = this.hpBarWidth;
		return levelConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.GetFieldValue("id").ToInt();
		this.roleId = parser.GetFieldValue("roleId").ToInt();
		this.level = parser.GetFieldValue("level").ToInt();
		this.hpValue = parser.GetFieldValue("hpValue").ToInt();
		this.attackValue = parser.GetFieldValue("attackValue").ToInt();
		this.defenseValue = parser.GetFieldValue("defenseValue").ToInt();
		this.exp = parser.GetFieldValue("exp").ToInt();
		this.attackSpeed = parser.GetFieldValue("attackSpeed").ToFloat();
		this.criticalValue = parser.GetFieldValue("criticalValue").ToInt();
		this.moveSpeed = parser.GetFieldValue("moveSpeed").ToFloat();
		this.jumpForce = parser.GetFieldValue("jumpForce").ToVector2();
		this.hpBarWidth = parser.GetFieldValue("hpBarWidth").ToFloat();
	}
}
