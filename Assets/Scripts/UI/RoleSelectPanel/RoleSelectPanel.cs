/*******************************************************/
/**2020-7-2 16:19****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleSelectPanel : BasePanel<RoleSelectPanelComponent, RoleSelectPanelSettings>
{
    protected override void OnInit(object arg)
    {
        m_Component.roleContentGroupView.onItemUpdateEvent += OnItemUpdate;
        m_Component.roleContentGroupView.onItemSelectEvent += OnItemSelect;
    }

    protected override void OnOpen()
    {
        m_HasSelect = true;
        m_Component.imgSelectRect.gameObject.SetActiveSelf(true);
        m_Component.roleContentGroupView.Update(ConfigDataSheet.roleSelectConfigDatas.Length);
        m_Component.roleContentGroupView.SelectItem(0);

        if (UIMgr.instance.IsOpen(UINames.LoadPanel))
        {
            LoadPanelMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
        }
        else
        {
            OnFadeWhiteComplete();
        }
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

            m_Component.roleContentGroupView.SelectItem(m_CurrSelectIndex);
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnSelect));
        }

        if (m_CurrSelectIndex != -1 && (InputMgr.instance.GetKeyDown(KeyType.A, true) || InputMgr.instance.GetKeyDown(KeyType.X, true)))
        {
            m_HasSelect = true;
            EnterStage();
        }
    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {
        m_Component.roleContentGroupView.onItemUpdateEvent -= OnItemUpdate;
        m_Component.roleContentGroupView.onItemSelectEvent -= OnItemSelect;
    }

    private void OnItemUpdate(RoleSelectPanelComponent.RoleContentItem item)
    {
        RoleSelectConfigData roleSelectConfigData = ConfigDataSheet.roleSelectConfigDatas[item.itemIndex];

        item.txtName.SetLanguageTextKey(roleSelectConfigData.name);
        item.txtDesc.SetLanguageTextKey(roleSelectConfigData.desc);
        item.imgRoleIcon.SetSprite(roleSelectConfigData.headIcon);
    }

    private void OnItemSelect(RoleSelectPanelComponent.RoleContentItem item, bool isSelect)
    {
        if (isSelect)
        {
            m_Component.imgSelectRect.SetParent(item.imgRoleIcon.transform, false);
            m_Component.imgSelectRect.anchoredPosition = Vector2.zero;
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
        m_Component.imgSelectRect.GetComponent<UIFrameEffect>().StopFrame();
        AudioMgr.instance.StopBgm(true);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.OnSelected));
        PlayerMgr.instance.selectRoleId = ConfigDataSheet.roleSelectConfigDatas[m_CurrSelectIndex].roleId;
        LoadPanelMgr.instance.DOFadeBlack(OnFadeBlackComplete);
    }

    private void OnFadeBlackComplete()
    {
        CloseSelf();
        UIMgr.instance.Open(UINames.StagePanel);
    }

    private bool m_HasSelect = false;
    private int m_CurrSelectIndex = -1;
}