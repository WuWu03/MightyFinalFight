/*
 * @Desc: Role.xlsx 数据表，SheetName: Role
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

public class RoleConfigData : BaseConfigData
{
	/// <summary>
	/// 名字
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// 资源路径
	/// </summary>
	public string assetName { get; private set; }

	/// <summary>
	/// 被击特效
	/// </summary>
	public string hitEffect { get; private set; }

	/// <summary>
	/// 头像
	/// </summary>
	public string headIcon { get; private set; }

	/// <summary>
	/// 攻速
	/// </summary>
	public float attackSpeed { get; private set; }

	/// <summary>
	/// 移速
	/// </summary>
	public float moveSpeed { get; private set; }

	/// <summary>
	/// 跳跃力
	/// </summary>
	public Vector2 jumpForce { get; private set; }

	/// <summary>
	/// 普攻
	/// </summary>
	public int[] attactIds { get; private set; }

	/// <summary>
	/// 跳跃攻击
	/// </summary>
	public int[] jumpAttackIds { get; private set; }

	/// <summary>
	/// 捕捉攻击
	/// </summary>
	public int catchAttackId { get; private set; }

	/// <summary>
	/// 扔出攻击
	/// </summary>
	public int throwAttackId { get; private set; }

	/// <summary>
	/// 武器攻击
	/// </summary>
	public int weaponAttackId { get; private set; }

	/// <summary>
	/// 扔出武器
	/// </summary>
	public int throwWeaponId { get; private set; }

	/// <summary>
	/// 技能
	/// </summary>
	public int[] skillIds { get; private set; }

	/// <summary>
	/// 武器
	/// </summary>
	public int weaponId { get; private set; }

	/// <summary>
	/// 可以被捉
	/// </summary>
	public bool isCatchControl { get; private set; }

	/// <summary>
	/// 行为树
	/// </summary>
	public int[] behaviourTreeIds { get; private set; }

	/// <summary>
	/// 被击动画
	/// </summary>
	public string[] hurtAnims { get; private set; }

	/// <summary>
	/// 是否是boss
	/// </summary>
	public bool isBoss { get; private set; }

	public RoleConfigData Clone()
	{
		RoleConfigData roleConfigData = new();
		{
			name = this.name;
			assetName = this.assetName;
			hitEffect = this.hitEffect;
			headIcon = this.headIcon;
			attackSpeed = this.attackSpeed;
			moveSpeed = this.moveSpeed;
			jumpForce = this.jumpForce;
			attactIds = this.attactIds;
			jumpAttackIds = this.jumpAttackIds;
			catchAttackId = this.catchAttackId;
			throwAttackId = this.throwAttackId;
			weaponAttackId = this.weaponAttackId;
			throwWeaponId = this.throwWeaponId;
			skillIds = this.skillIds;
			weaponId = this.weaponId;
			isCatchControl = this.isCatchControl;
			behaviourTreeIds = this.behaviourTreeIds;
			hurtAnims = this.hurtAnims;
			isBoss = this.isBoss;
		}

		return roleConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		id = parser.Read<int>();
		name = parser.Read<string>();
		assetName = parser.Read<string>();
		hitEffect = parser.Read<string>();
		headIcon = parser.Read<string>();
		attackSpeed = parser.Read<float>();
		moveSpeed = parser.Read<float>();
		jumpForce = parser.Read<Vector2>();
		attactIds = parser.Read<int[]>();
		jumpAttackIds = parser.Read<int[]>();
		catchAttackId = parser.Read<int>();
		throwAttackId = parser.Read<int>();
		weaponAttackId = parser.Read<int>();
		throwWeaponId = parser.Read<int>();
		skillIds = parser.Read<int[]>();
		weaponId = parser.Read<int>();
		isCatchControl = parser.Read<bool>();
		behaviourTreeIds = parser.Read<int[]>();
		hurtAnims = parser.Read<string[]>();
		isBoss = parser.Read<bool>();
	}
}