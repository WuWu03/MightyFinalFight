/*
 * @Desc: SceneItem.xlsx 数据表，SheetName: SceneItem
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

public class SceneItemConfigData : BaseConfigData
{
	/// <summary>
	/// 名字
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// 资源
	/// </summary>
	public string assetName { get; private set; }

	/// <summary>
	/// 类型
	/// </summary>
	public int type { get; private set; }

	/// <summary>
	/// 生命或经验
	/// </summary>
	public int value { get; private set; }

	/// <summary>
	/// 武器是否可以掉落
	/// </summary>
	public bool canDrop { get; private set; }

	public SceneItemConfigData Clone()
	{
		SceneItemConfigData sceneItemConfigData = new();
		{
			name = this.name;
			assetName = this.assetName;
			type = this.type;
			value = this.value;
			canDrop = this.canDrop;
		}

		return sceneItemConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		id = parser.Read<int>();
		name = parser.Read<string>();
		assetName = parser.Read<string>();
		type = parser.Read<int>();
		value = parser.Read<int>();
		canDrop = parser.Read<bool>();
	}
}