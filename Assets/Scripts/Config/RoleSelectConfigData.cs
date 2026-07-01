/*
 * @Desc: RoleSelect.xlsx 数据表，SheetName: RoleSelect
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
		RoleSelectConfigData roleSelectConfigData = new();
		{
			roleId = this.roleId;
			name = this.name;
			desc = this.desc;
			headIcon = this.headIcon;
			assetName = this.assetName;
			animName = this.animName;
			soundName = this.soundName;
			showTime = this.showTime;
			animSpeed = this.animSpeed;
		}

		return roleSelectConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		id = parser.Read<int>();
		roleId = parser.Read<int>();
		name = parser.Read<string>();
		desc = parser.Read<string>();
		headIcon = parser.Read<string>();
		assetName = parser.Read<string>();
		animName = parser.Read<string>();
		soundName = parser.Read<string>();
		showTime = parser.Read<float>();
		animSpeed = parser.Read<float>();
	}
}