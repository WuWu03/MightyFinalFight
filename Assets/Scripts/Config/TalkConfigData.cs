
//===================================================
//作者：WuWu                                          
//创建时间：2024-06-05 11:58:34
//备注：此代码为工具生成 请勿手工修改
//===================================================
using GameFrameWork;
using GameFrameWork.ConfigData;
using LitJson;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Talk.xlsx数据表
/// SheetName:Sheet1
/// </summary>
public class TalkConfigData : BaseConfigData
{
	public class TalkSelect
	{
		public class Effect
		{
			public int effectId { get; set; }
			public int effectValue { get; set; }
		}

		public string content { get; set; }
		public int talkId { get; set; }
		public Effect effect { get; set; }
	}
	/// <summary>
	/// 叙述角色
	/// </summary>
	public int roleId { get; private set; }

	/// <summary>
	/// 内容
	/// </summary>
	public string content { get; private set; }

	/// <summary>
	/// 对话选项
	/// </summary>
	public TalkSelect[] talkSelect { get; private set; }

	/// <summary>
	/// 下一句
	/// </summary>
	public int nextTalkId { get; private set; }

	public TalkConfigData Clone()
	{
		TalkConfigData talkConfigData = new TalkConfigData();
		talkConfigData.roleId = this.roleId;
		talkConfigData.content = this.content;
		talkConfigData.talkSelect = this.talkSelect;
		talkConfigData.nextTalkId = this.nextTalkId;
		return talkConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.GetFieldValue("id").ToInt();
		this.roleId = parser.GetFieldValue("roleId").ToInt();
		this.content = parser.GetFieldValue("content");
		this.talkSelect = JsonMapper.ToObject<TalkSelect[]>(parser.GetFieldValue("talkSelect"));
		this.nextTalkId = parser.GetFieldValue("nextTalkId").ToInt();
	}
}
