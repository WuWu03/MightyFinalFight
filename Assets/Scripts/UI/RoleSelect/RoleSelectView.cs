/*
 * @Desc: RoleSelect 模块 RoleSelectView 界面组件
 * @Date: 2025-11-26 17:05:58
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class RoleSelectView : UIBaseView<RoleSelectView, RoleSelectViewPresenter, RoleSelectViewSettings>
{
    //roleSelect,StaticList
    public StaticList roleSelectList { get; private set; }
    //imgSelect,RectTransform
    public RectTransform imgSelectRect { get; private set; }

    protected override void OnInitView(UIRefRoot root)
    {
        roleSelectList = root.objects[0] as StaticList;
        roleSelectList?.Init<RoleSelectListItem>();
        imgSelectRect = root.objects[1] as RectTransform;
    }

    public class RoleSelectListItem : BaseListItem
    {
        //roleSelect/roleSelectListItem/imgRoleIcon,ImageEx
        public ImageEx imgRoleIcon { get; private set; }
        //roleSelect/roleSelectListItem/txtName,LanguageText
        public LanguageText txtName { get; private set; }
        //roleSelect/roleSelectListItem/txtDesc,LanguageText
        public LanguageText txtDesc { get; private set; }
        protected override void OnCreate(GameObject go)
        {
            UIRefRoot uiRefRoot = go.GetComponent<UIRefRoot>();
            imgRoleIcon = uiRefRoot.objects[0] as ImageEx;
            txtName = uiRefRoot.objects[1] as LanguageText;
            txtDesc = uiRefRoot.objects[2] as LanguageText;
        }
    }
}