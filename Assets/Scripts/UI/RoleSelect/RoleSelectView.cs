/*
 * @Desc: RoleSelect 模块 RoleSelectView 界面视图
 * @Date: 2020-7-2 16:19:21
 * @Author: WuWu
 */

using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleSelectView : UIBaseView<RoleSelectViewComponent, RoleSelectViewSettings>
{
    protected override void OnOpen(object arg)
    {
        component.roleContentGroupView.onItemUpdateEvent += OnItemUpdate;
        component.roleContentGroupView.onItemSelectEvent += OnItemSelect;
    }

    protected override void OnShow(object arg)
    {
        m_HasSelect = true;
        component.imgSelectRect.gameObject.SetActiveSelf(true);
        component.roleContentGroupView.Update(ConfigDataSheet.roleSelectConfigDatas.Length);
        component.roleContentGroupView.SelectItem(0);
        LoadMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
    }

    protected override void OnUpdate()
    {
        if (m_HasSelect)
        {
            return;
        }

        Vector2 axis = InputMgr.instance.GetAxis(AxisType.LeftAxis);

        if (axis.y != 0)
        {
            if (axis.y < 0)
            {
                m_CurrSelectIndex++;
                if (m_CurrSelectIndex >= ConfigDataSheet.roleSelectConfigDatas.Length) m_CurrSelectIndex = 0;
            }
            else
            {
                m_CurrSelectIndex--;
                if (m_CurrSelectIndex < 0) m_CurrSelectIndex = ConfigDataSheet.roleSelectConfigDatas.Length - 1;
            }

            component.roleContentGroupView.SelectItem(m_CurrSelectIndex);
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnSelect));
        }

        if (m_CurrSelectIndex != -1 && (InputMgr.instance.GetKeyDown(KeyType.A, true) || InputMgr.instance.GetKeyDown(KeyType.X, true)))
        {
            m_HasSelect = true;
            EnterStage();
        }
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {
        component.roleContentGroupView.onItemUpdateEvent -= OnItemUpdate;
        component.roleContentGroupView.onItemSelectEvent -= OnItemSelect;
    }

    private void OnItemUpdate(RoleSelectViewComponent.RoleContentItem item)
    {
        RoleSelectConfigData roleSelectConfigData = ConfigDataSheet.roleSelectConfigDatas[item.itemIndex];

        item.txtName.SetLanguageTextKey(roleSelectConfigData.name);
        item.txtDesc.SetLanguageTextKey(roleSelectConfigData.desc);
        item.imgRoleIcon.SetSprite(roleSelectConfigData.headIcon);
    }

    private void OnItemSelect(RoleSelectViewComponent.RoleContentItem item, bool isSelect)
    {
        if (isSelect)
        {
            component.imgSelectRect.SetParent(item.imgRoleIcon.transform, false);
            component.imgSelectRect.anchoredPosition = Vector2.zero;
            m_CurrSelectIndex = item.itemIndex;
        }
    }

    private void OnFadeWhiteComplete()
    {
        m_HasSelect = false;
        AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmCharacter_Start), false);
        AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmCharacter_Loop), true);
    }

    private void EnterStage()
    {
        component.imgSelectRect.GetComponent<UIFrameEffect>().StopFrame();
        AudioMgr.instance.StopBgm(true);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnSelected));
        PlayerMgr.instance.selectRoleId = ConfigDataSheet.roleSelectConfigDatas[m_CurrSelectIndex].roleId;
        LoadMgr.instance.DOFadeBlack(OnFadeBlackComplete);
    }

    private void OnFadeBlackComplete()
    {
        CloseSelf();
        UIMgr.instance.Open<StageView>();
    }

    private bool m_HasSelect = false;
    private int m_CurrSelectIndex = -1;
}