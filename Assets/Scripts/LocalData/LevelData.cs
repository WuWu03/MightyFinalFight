
//===================================================
//作者：GQY                                          
//创建时间：2022-09-07 16:36:31
//备注：此代码为工具生成 请勿手工修改
//===================================================
using GameFrameWork;
using GameFrameWork.LocalData;
using LitJson;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Level.xlsx数据表
/// SheetName:Sheet1
/// </summary>
public class LevelData : BaseLocalData
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

	public LevelData Clone()
	{
		LevelData levelData = new LevelData();
		levelData.roleId = this.roleId;
		levelData.level = this.level;
		levelData.hpValue = this.hpValue;
		levelData.attackValue = this.attackValue;
		levelData.defenseValue = this.defenseValue;
		levelData.exp = this.exp;
		levelData.attackSpeed = this.attackSpeed;
		levelData.criticalValue = this.criticalValue;
		levelData.moveSpeed = this.moveSpeed;
		levelData.jumpForce = this.jumpForce;
		levelData.hpBarWidth = this.hpBarWidth;
		return levelData;
	}

	public override void Read(LocalDataParser parser)
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
