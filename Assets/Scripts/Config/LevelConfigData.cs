/*
 * @Desc: Level.xlsx 数据表，SheetName: Level
 * @Date: 2026-06-29 15:07:52
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework;
using WuWuFramework.ConfigData;
using LitJson;
using System;
using System.Collections;
using UnityEngine;

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
		this.id = parser.ReadInt();
		this.roleId = parser.ReadInt();
		this.level = parser.ReadInt();
		this.hpValue = parser.ReadInt();
		this.attackValue = parser.ReadInt();
		this.defenseValue = parser.ReadInt();
		this.exp = parser.ReadInt();
		this.attackSpeed = parser.ReadFloat();
		this.criticalValue = parser.ReadInt();
		this.moveSpeed = parser.ReadFloat();
		this.jumpForce = parser.ReadVector2();
		this.hpBarWidth = parser.ReadFloat();
	}
}