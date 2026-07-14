/*
 * @Desc: RoleSelect 模块 RoleSelectView 视图展示器
 * @Date: 2020-7-2 16:19:21
 * @Author: WuWu
 */

using WuWuFramework;
using WuWuFramework.Input;
using WuWuFramework.UI;
using WuWuFramework.Utils;
using UnityEngine;

public class RoleSelectViewPresenter : UIBaseViewPresenter<RoleSelectView>
{
    private bool m_HasSelect;
    private int m_CurrSelectIndex = -1;
    private RoleSelectConfigData[] m_RoleSelectConfigData;

    protected override void OnOpen(object arg)
    {
        view.roleSelectList.itemUpdateEvent += OnItemUpdate;
        view.roleSelectList.itemSelectEvent += OnItemSelect;
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.LeftAxis, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.DPad, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.A, InputEventCallType.Performed, Select);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.LeftAxis, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.DPad, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.xboxInputController.AddInputEvent(XboxInputKey.A, InputEventCallType.Performed, Select);
    }

    protected override void OnShow(object arg)
    {
        m_HasSelect = true;
        m_RoleSelectConfigData = GameEntry.configDataMgr.Get<RoleSelectConfigData>();
        view.imgSelectRect.gameObject.SetActiveSelf(true);
        view.roleSelectList.SetItemCount(m_RoleSelectConfigData.Length);
        view.roleSelectList.SelectItem(0);
        LoadMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        m_RoleSelectConfigData = null;
    }

    protected override void OnClose()
    {
        GameEntry.inputMgr.keyBoardInputController.RemoveInputEvent(KeyboardInputKey.LeftAxis, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.keyBoardInputController.RemoveInputEvent(KeyboardInputKey.DPad, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.keyBoardInputController.RemoveInputEvent(KeyboardInputKey.A, InputEventCallType.Performed, Select);
        GameEntry.inputMgr.xboxInputController.RemoveInputEvent(XboxInputKey.LeftAxis, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.xboxInputController.RemoveInputEvent(XboxInputKey.DPad, InputEventCallType.Performed, ChangeSelect);
        GameEntry.inputMgr.xboxInputController.RemoveInputEvent(XboxInputKey.A, InputEventCallType.Performed, Select);
    }

    protected override void OnDestroy()
    {
        view.roleSelectList.itemUpdateEvent -= OnItemUpdate;
        view.roleSelectList.itemSelectEvent -= OnItemSelect;
    }

    private void OnItemUpdate(BaseListItem item)
    {
        if (item is RoleSelectView.RoleSelectListItem roleSelectListItem)
        {
            RoleSelectConfigData roleSelectConfigData = m_RoleSelectConfigData[item.index];
            roleSelectListItem.txtName.SetLanguageTextKey(roleSelectConfigData.name);
            roleSelectListItem.txtDesc.SetLanguageTextKey(roleSelectConfigData.desc);
            roleSelectListItem.imgRoleIcon.spriteName = roleSelectConfigData.headIcon;
        }
    }

    private void OnItemSelect(BaseListItem item, bool isSelect)
    {
        if (isSelect && item is RoleSelectView.RoleSelectListItem roleSelectListItem)
        {
            view.imgSelectRect.SetParent(roleSelectListItem.imgRoleIcon.transform, false);
            view.imgSelectRect.anchoredPosition = Vector2.zero;
            m_CurrSelectIndex = item.index;
        }
    }

    private void OnFadeWhiteComplete()
    {
        m_HasSelect = false;
        GameEntry.soundMgr.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmCharacter_Start), false);
        GameEntry.soundMgr.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmCharacter_Loop), true);
    }


    private void ChangeSelect(Vector2 axis)
    {
        float y = axis.y;

        if (m_HasSelect || y == 0)
        {
            return;
        }

        if (y < 0)
        {
            m_CurrSelectIndex++;
        }
        else
        {
            m_CurrSelectIndex--;
        }

        if (m_CurrSelectIndex >= m_RoleSelectConfigData.Length)
        {
            m_CurrSelectIndex = 0;
        }
        else if (m_CurrSelectIndex < 0)
        {
            m_CurrSelectIndex = m_RoleSelectConfigData.Length - 1;
        }

        view.roleSelectList.SelectItem(m_CurrSelectIndex);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnSelect));
    }


    private void Select()
    {
        if (m_CurrSelectIndex != -1)
        {
            m_HasSelect = true;
            EnterStage();
        }
    }

    private void EnterStage()
    {
        view.imgSelectRect.GetComponent<UIFrameEffect>().StopFrame();
        GameEntry.soundMgr.StopBgm(true);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnSelected));
        PlayerMgr.instance.selectRoleId = m_RoleSelectConfigData[m_CurrSelectIndex].roleId;
        LoadMgr.instance.DOFadeBlack(OnFadeBlackComplete);
    }

    private void OnFadeBlackComplete()
    {
        CloseSelf();
        GameEntry.uiMgr.Open<StageView>();
    }
}