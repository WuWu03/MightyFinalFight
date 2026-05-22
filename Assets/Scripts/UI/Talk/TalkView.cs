/*
 * @Desc: Talk 模块 TalkView 界面组件
 * @Date: 2025-11-26 17:08:14
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class TalkView : UIBaseView<TalkView, TalkViewPresenter, TalkViewSettings>
{
    //bottom/txtContent,LanguageText
    public LanguageText languageContent { get; private set; }
    //bottom/txtContent,TextMeshProUGUI
    public TextMeshProUGUI txtContent { get; private set; }
    //bottom/talkSelect,StaticList
    public StaticList talkSelectList { get; private set; }

    protected override void OnInitView(UIRefRoot root)
    {
        languageContent = root.objects[0] as LanguageText;
        txtContent = root.objects[1] as TextMeshProUGUI;
        talkSelectList = root.objects[2] as StaticList;
        talkSelectList?.Init<TalkSelectListItem>();
    }

    public class TalkSelectListItem : BaseListItem
    {
        //bottom/talkSelect/talkSelectItem/txtSelect,LanguageText
        public LanguageText txtSelect { get; private set; }
        //bottom/talkSelect/talkSelectItem/select,GameObject
        public GameObject selectGo { get; private set; }
        protected override void OnCreate(GameObject go)
        {
            UIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();
            txtSelect = uiRefRoot.objects[0] as LanguageText;
            selectGo = uiRefRoot.objects[1] as GameObject;
        }
    }
}