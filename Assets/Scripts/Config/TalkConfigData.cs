/*
 * @Desc: Talk.xlsx 数据表，SheetName: Talk
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
		TalkConfigData talkConfigData = new();
		{
			roleId = this.roleId;
			content = this.content;
			talkSelect = this.talkSelect;
			nextTalkId = this.nextTalkId;
		}

		return talkConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		id = parser.Read<int>();
		roleId = parser.Read<int>();
		content = parser.Read<string>();
		talkSelect = JsonMapper.ToObject<TalkSelect[]>(parser.Read<string>());
		nextTalkId = parser.Read<int>();
	}
}