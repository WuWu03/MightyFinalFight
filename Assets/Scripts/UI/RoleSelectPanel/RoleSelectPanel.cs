/*******************************************************/
/**2020-7-2 16:19****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.UI;
using System;
using UnityEngine;

public class RoleSelectPanel : BasePanel
{
    protected override Type componentType
    {
        get
        {
            return typeof(RoleSelectPanelComponent);
        }
    }

    protected override Type settingsType
    {
        get
        {
            return typeof(RoleSelectPanelSettings);
        }
    }

    protected override void OnInit(BasePanelComponent panelComponent, object[] param)
    {
        m_Component = panelComponent as RoleSelectPanelComponent;
        m_Component.roleContentGroupView.Init(m_Component.roleContent, m_Component.itemGO, 3);
        m_Component.roleContentGroupView.onItemUpdateEvent = OnItemUpdate;
        m_Component.roleContentGroupView.onItemSelectEvent = OnItemSelect;
    }

	protected override void OnOpen()
	{
		m_HasSelect = false;
		m_Component.imgSelectRect.gameObject.SetActive(true);
		m_Component.roleContentGroupView.Update(ConfigDataHelper.roleSelectConfigDatas.Length);
		m_Component.roleContentGroupView.SelectItem(0);

		AudioMgr.instance.PlayBGM(AssetPathDefine.AudioClipPath, SoundName.Bgm14Character, true);
	}

    protected override void OnUpdate()
    {
		if(m_HasSelect)
        {
			return;
        }

		Vector2 axis = InputMgr.instance.GetAxis(AxisType.LeftAxis, true);

		if (axis.y != 0)
		{
			if (axis.y < 0)
			{
				m_CurrSelectIndex++;
				if (m_CurrSelectIndex >= ConfigDataHelper.roleSelectConfigDatas.Length) m_CurrSelectIndex = 0;
			}
			else
			{
				m_CurrSelectIndex--;
				if (m_CurrSelectIndex < 0) m_CurrSelectIndex = ConfigDataHelper.roleSelectConfigDatas.Length - 1;
			}

			m_Component.roleContentGroupView.SelectItem(m_CurrSelectIndex);
			AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, SoundName.OnSelect);
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

	}

	private void OnItemUpdate(RoleSelectPanelComponent.RoleContentItem item)
	{
		RoleSelectConfigData roleSelectConfigData = ConfigDataHelper.roleSelectConfigDatas[item.itemIndex];

		item.txtName.UpdateLanguageTextKey(roleSelectConfigData.name);
		item.txtDesc.UpdateLanguageTextKey(roleSelectConfigData.desc);
		item.btnRoleIcon.image.SetSprite(roleSelectConfigData.headIcon);
	}

	private void OnItemSelect(RoleSelectPanelComponent.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			m_Component.imgSelectRect.SetParent(item.btnRoleIcon.transform, false);
			m_Component.imgSelectRect.localPosition = Vector3.zero;
			m_CurrSelectIndex = item.itemIndex;
		}
	}

	private void EnterStage()
	{
        m_Component.imgSelectRect.GetComponent<UIFrameEffect>().StopFrame();

		AudioMgr.instance.StopBGM(true);
		AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, SoundName.OnSelected);
		PlayerMgr.instance.selectRoleId = ConfigDataHelper.roleSelectConfigDatas[m_CurrSelectIndex].roleId;

		LoadPanel loadPanel = UIMgr.instance.Open<LoadPanel>();
		loadPanel.DOFade(0f, 1f, 0.3f, 0.5f, () =>
		{
			UIMgr.instance.Open<StagePanel>();
		});

        loadPanel.DOFade(1, 0, 0.3f, 0.1f, () =>
        {
            UIMgr.instance.Close<LoadPanel>();
            CloseSelf();
        });
    }

	private bool m_HasSelect = false;
	private int m_CurrSelectIndex = -1;
	private RoleSelectPanelComponent m_Component = null;
}