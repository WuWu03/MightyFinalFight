/*
 * @Desc: Talk.xlsx数据表
 * @Date: 2024-06-06 11:09:24
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework;
using WuWuFramework.ConfigData;
using LitJson;

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
