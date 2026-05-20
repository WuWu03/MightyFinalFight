/*
 * @Desc: RoleSelect 模块 RoleSelectPresenter 界面视图
 * @Date: 2020-7-2 16:19:21
 * @Author: GQY
 */

using GameFrameWork;
using GameFrameWork.Input;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleSelectPresenter : UIBasePresenter<RoleSelectView, RoleSelectViewSettings>
{
    private bool m_HasSelect;
    private int m_CurrSelectIndex = -1;
    private RoleSelectConfigData[] m_RoleSelectConfigData;
    protected override void OnOpen(object arg)
    {
        view.roleSelectList.itemUpdateEvent += OnItemUpdate;
        view.roleSelectList.itemSelectEvent += OnItemSelect;
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
        if (m_HasSelect)
        {
            return;
        }

        Vector2 axis = GameEntry.inputMgr.GetAxis(AxisType.LeftAxis);

        if (axis.y != 0)
        {
            if (axis.y < 0)
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

        if (m_CurrSelectIndex != -1 && (GameEntry.inputMgr.GetKeyDown(KeyType.A, true) || GameEntry.inputMgr.GetKeyDown(KeyType.X, true)))
        {
            m_HasSelect = true;
            EnterStage();
        }
    }

    protected override void OnHide()
    {
        m_RoleSelectConfigData = null;
    }

    protected override void OnClose()
    {

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
        GameFrameWorkMgr.GetModule<IUIMgr>().Open<StagePresenter>();
    }
}