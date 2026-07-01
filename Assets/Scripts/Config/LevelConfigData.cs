/*
 * @Desc: Level.xlsx 数据表，SheetName: Level
 * @Date: 2026-07-01 09:58:10
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuWuFramework;
using WuWuFramework.ConfigData;

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
		LevelConfigData levelConfigData = new();
		{
			roleId = this.roleId;
			level = this.level;
			hpValue = this.hpValue;
			attackValue = this.attackValue;
			defenseValue = this.defenseValue;
			exp = this.exp;
			attackSpeed = this.attackSpeed;
			criticalValue = this.criticalValue;
			moveSpeed = this.moveSpeed;
			jumpForce = this.jumpForce;
			hpBarWidth = this.hpBarWidth;
		}

		return levelConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		id = parser.Read<int>();
		roleId = parser.Read<int>();
		level = parser.Read<int>();
		hpValue = parser.Read<int>();
		attackValue = parser.Read<int>();
		defenseValue = parser.Read<int>();
		exp = parser.Read<int>();
		attackSpeed = parser.Read<float>();
		criticalValue = parser.Read<int>();
		moveSpeed = parser.Read<float>();
		jumpForce = parser.Read<Vector2>();
		hpBarWidth = parser.Read<float>();
	}
}