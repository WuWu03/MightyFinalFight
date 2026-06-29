/*
 * @Desc: RoleSelect.xlsx 数据表，SheetName: RoleSelect
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
		this.id = parser.ReadInt();
		this.roleId = parser.ReadInt();
		this.name = parser.ReadUTF8String();
		this.desc = parser.ReadUTF8String();
		this.headIcon = parser.ReadUTF8String();
		this.assetName = parser.ReadUTF8String();
		this.animName = parser.ReadUTF8String();
		this.soundName = parser.ReadUTF8String();
		this.showTime = parser.ReadFloat();
		this.animSpeed = parser.ReadFloat();
	}
}