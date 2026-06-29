/*
 * @Desc: SceneItem.xlsx 数据表，SheetName: SceneItem
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
		SceneItemConfigData sceneItemConfigData = new SceneItemConfigData();
		sceneItemConfigData.name = this.name;
		sceneItemConfigData.assetName = this.assetName;
		sceneItemConfigData.type = this.type;
		sceneItemConfigData.value = this.value;
		sceneItemConfigData.canDrop = this.canDrop;
		return sceneItemConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.ReadInt();
		this.name = parser.ReadUTF8String();
		this.assetName = parser.ReadUTF8String();
		this.type = parser.ReadInt();
		this.value = parser.ReadInt();
		this.canDrop = parser.ReadBool();
	}
}