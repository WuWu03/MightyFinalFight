/*
 * @Desc: Story.xlsx 数据表，SheetName: Story
 * @Date: 2026-06-29 16:40:22
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WuWuFramework;
using WuWuFramework.ConfigData;
using static UnityEngine.Rendering.DebugUI;

public class StoryConfigData : BaseConfigData
{
    /// <summary>
    /// 剧情编号
    /// </summary>
    public int storyId { get; private set; }

    /// <summary>
    /// 剧情轨道
    /// </summary>
    public int track { get; private set; }

    /// <summary>
    /// 剧情命令
    /// </summary>
    public string storyContent { get; private set; }

    /// <summary>
    /// 测试
    /// </summary>
    public Dictionary<int, string> test { get; private set; }

    public StoryConfigData Clone()
    {
        StoryConfigData storyConfigData = new StoryConfigData();
        storyConfigData.storyId = this.storyId;
        storyConfigData.track = this.track;
        storyConfigData.storyContent = this.storyContent;
        storyConfigData.test = this.test;
        return storyConfigData;
    }

    public override void Read(ConfigDataParser parser)
    {
        this.id = parser.ReadInt();
        this.storyId = parser.ReadInt();
        this.track = parser.ReadInt();
        this.storyContent = parser.ReadUTF8String();
        this.test = parser.ReadDictionary<int, string>();
    }
}