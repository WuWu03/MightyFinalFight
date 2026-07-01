/*
 * @Desc: Story.xlsx 数据表，SheetName: Story
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
        StoryConfigData storyConfigData = new();
        {
            storyId = this.storyId;
            track = this.track;
            storyContent = this.storyContent;
            test = this.test;
        }

        return storyConfigData;
    }

    public override void Read(ConfigDataParser parser)
    {
        id = parser.Read<int>();
        storyId = parser.Read<int>();
        track = parser.Read<int>();
        storyContent = parser.Read<string>();
        test = parser.ReadDictionary<int, string>();
    }
}