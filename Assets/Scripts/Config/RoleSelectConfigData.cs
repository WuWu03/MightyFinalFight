
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
/// RoleSelect.xlsx数据表
/// SheetName:Sheet1
/// </summary>
public class RoleSelectConfigData : BaseConfigData
{
	/// <summary>
	/// 角色id
	/// </summary>
	public int roleId { get; private set; }

	/// <summary>
	/// 名字
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// 描述
	/// </summary>
	public string desc { get; private set; }

	/// <summary>
	/// 头像
	/// </summary>
	public string headIcon { get; private set; }

	/// <summary>
	/// 资源路径
	/// </summary>
	public string assetName { get; private set; }

	/// <summary>
	/// 动画
	/// </summary>
	public string animName { get; private set; }

	/// <summary>
	/// 展示界面音效
	/// </summary>
	public string soundName { get; private set; }

	/// <summary>
	/// 展示时间
	/// </summary>
	public float showTime { get; private set; }

	/// <summary>
	/// 动画速度
	/// </summary>
	public float animSpeed { get; private set; }

	public RoleSelectConfigData Clone()
	{
		RoleSelectConfigData roleSelectConfigData = new RoleSelectConfigData();
		roleSelectConfigData.roleId = this.roleId;
		roleSelectConfigData.name = this.name;
		roleSelectConfigData.desc = this.desc;
		roleSelectConfigData.headIcon = this.headIcon;
		roleSelectConfigData.assetName = this.assetName;
		roleSelectConfigData.animName = this.animName;
		roleSelectConfigData.soundName = this.soundName;
		roleSelectConfigData.showTime = this.showTime;
		roleSelectConfigData.animSpeed = this.animSpeed;
		return roleSelectConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.GetFieldValue("id").ToInt();
		this.roleId = parser.GetFieldValue("roleId").ToInt();
		this.name = parser.GetFieldValue("name");
		this.desc = parser.GetFieldValue("desc");
		this.headIcon = parser.GetFieldValue("headIcon");
		this.assetName = parser.GetFieldValue("assetName");
		this.animName = parser.GetFieldValue("animName");
		this.soundName = parser.GetFieldValue("soundName");
		this.showTime = parser.GetFieldValue("showTime").ToFloat();
		this.animSpeed = parser.GetFieldValue("animSpeed").ToFloat();
	}
}
