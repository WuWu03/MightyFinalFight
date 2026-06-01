/*
 * @Desc: Load 模块 LoadView 界面组件
 * @Date: 2025-11-26 16:35:53
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WuWuFramework.UI;

public class LoadView : UIBaseView<LoadView, LoadPresenter, LoadViewSettings>
{
    //imgShade,Image
    public Image imgShade { get; private set; }

    protected override void OnInitView(UIRefRoot root)
    {
        imgShade = root.objects[0] as Image;
    }
}