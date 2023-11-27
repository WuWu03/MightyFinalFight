
//===================================================
//作者：WuWu                                          
//创建时间：2023-07-17 15:39:12
//备注：此代码为工具生成 请勿手工修改
//===================================================
using GameFrameWork;
using GameFrameWork.ConfigData;
using LitJson;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Role.xlsx数据表
/// SheetName:Sheet1
/// </summary>
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
	/// 连击等待时间
	/// </summary>
	public float[] attackWait { get; private set; }

	/// <summary>
	/// 下一击等待时间
	/// </summary>
	public float[] attackNextTime { get; private set; }

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
		roleConfigData.attackWait = this.attackWait;
		roleConfigData.attackNextTime = this.attackNextTime;
		roleConfigData.weaponId = this.weaponId;
		roleConfigData.isCatchControl = this.isCatchControl;
		roleConfigData.behaviourTreeIds = this.behaviourTreeIds;
		roleConfigData.hurtAnims = this.hurtAnims;
		roleConfigData.isBoss = this.isBoss;
		return roleConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.GetFieldValue("id").ToInt();
		this.name = parser.GetFieldValue("name");
		this.assetName = parser.GetFieldValue("assetName");
		this.hitEffect = parser.GetFieldValue("hitEffect");
		this.headIcon = parser.GetFieldValue("headIcon");
		this.attackSpeed = parser.GetFieldValue("attackSpeed").ToFloat();
		this.moveSpeed = parser.GetFieldValue("moveSpeed").ToFloat();
		this.jumpForce = parser.GetFieldValue("jumpForce").ToVector2();
		this.attactIds = parser.GetFieldValue("attactIds").ToIntArray();
		this.jumpAttackIds = parser.GetFieldValue("jumpAttackIds").ToIntArray();
		this.catchAttackId = parser.GetFieldValue("catchAttackId").ToInt();
		this.throwAttackId = parser.GetFieldValue("throwAttackId").ToInt();
		this.weaponAttackId = parser.GetFieldValue("weaponAttackId").ToInt();
		this.throwWeaponId = parser.GetFieldValue("throwWeaponId").ToInt();
		this.skillIds = parser.GetFieldValue("skillIds").ToIntArray();
		this.attackWait = parser.GetFieldValue("attackWait").ToFloatArray();
		this.attackNextTime = parser.GetFieldValue("attackNextTime").ToFloatArray();
		this.weaponId = parser.GetFieldValue("weaponId").ToInt();
		this.isCatchControl = parser.GetFieldValue("isCatchControl").ToBool();
		this.behaviourTreeIds = parser.GetFieldValue("behaviourTreeIds").ToIntArray();
		this.hurtAnims = parser.GetFieldValue("hurtAnims").ToStringArray();
		this.isBoss = parser.GetFieldValue("isBoss").ToBool();
	}
}
