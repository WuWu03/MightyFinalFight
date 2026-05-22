/*
 * @Desc: Version 模块 VersionView 界面数据
 * @Date: 2025-10-11 12:36:11
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class VersionView : UIBaseView<VersionView, VersionViewPresenter, VersionViewSettings>
{
    //txtVersion,LanguageText
    public LanguageText txtVersion { get; private set; }

    protected override void OnInitView(UIRefRoot root)
    {
        txtVersion = root.objects[0] as LanguageText;
    }
}