
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
/// Localization.xlsx数据表
/// SheetName:Sheet1
/// </summary>
public class LocalizationConfigData : BaseConfigData
{
	/// <summary>
	/// 语言key
	/// </summary>
	public string key { get; private set; }

	/// <summary>
	/// 英语
	/// </summary>
	public string english { get; private set; }

	/// <summary>
	/// 简体中文
	/// </summary>
	public string simplifiedChinese { get; private set; }

	/// <summary>
	/// 日语
	/// </summary>
	public string japanese { get; private set; }

	public LocalizationConfigData Clone()
	{
		LocalizationConfigData localizationConfigData = new LocalizationConfigData();
		localizationConfigData.key = this.key;
		localizationConfigData.english = this.english;
		localizationConfigData.simplifiedChinese = this.simplifiedChinese;
		localizationConfigData.japanese = this.japanese;
		return localizationConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.GetFieldValue("id").ToInt();
		this.key = parser.GetFieldValue("key");
		this.english = parser.GetFieldValue("english");
		this.simplifiedChinese = parser.GetFieldValue("simplifiedChinese");
		this.japanese = parser.GetFieldValue("japanese");
	}
}
