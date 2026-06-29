/*
 * @Desc: Role.xlsx 数据表，SheetName: Role
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
		RoleConfigData roleConfigData = new RoleConfigData();
		roleConfigData.name = this.name;
		roleConfigData.assetName = this.assetName;
		roleConfigData.hitEffect = this.hitEffect;
		roleConfigData.headIcon = this.headIcon;
		roleConfigData.attackSpeed = this.attackSpeed;
		roleConfigData.moveSpeed = this.moveSpeed;
		roleConfigData.jumpForce = this.jumpForce;
		roleConfigData.attactIds = this.attactIds;
		roleConfigData.jumpAttackIds = this.jumpAttackIds;
		roleConfigData.catchAttackId = this.catchAttackId;
		roleConfigData.throwAttackId = this.throwAttackId;
		roleConfigData.weaponAttackId = this.weaponAttackId;
		roleConfigData.throwWeaponId = this.throwWeaponId;
		roleConfigData.skillIds = this.skillIds;
		roleConfigData.weaponId = this.weaponId;
		roleConfigData.isCatchControl = this.isCatchControl;
		roleConfigData.behaviourTreeIds = this.behaviourTreeIds;
		roleConfigData.hurtAnims = this.hurtAnims;
		roleConfigData.isBoss = this.isBoss;
		return roleConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.ReadInt();
		this.name = parser.ReadUTF8String();
		this.assetName = parser.ReadUTF8String();
		this.hitEffect = parser.ReadUTF8String();
		this.headIcon = parser.ReadUTF8String();
		this.attackSpeed = parser.ReadFloat();
		this.moveSpeed = parser.ReadFloat();
		this.jumpForce = parser.ReadVector2();
		this.attactIds = parser.ReadIntArray();
		this.jumpAttackIds = parser.ReadIntArray();
		this.catchAttackId = parser.ReadInt();
		this.throwAttackId = parser.ReadInt();
		this.weaponAttackId = parser.ReadInt();
		this.throwWeaponId = parser.ReadInt();
		this.skillIds = parser.ReadIntArray();
		this.weaponId = parser.ReadInt();
		this.isCatchControl = parser.ReadBool();
		this.behaviourTreeIds = parser.ReadIntArray();
		this.hurtAnims = parser.ReadUTF8StringArray();
		this.isBoss = parser.ReadBool();
	}
}