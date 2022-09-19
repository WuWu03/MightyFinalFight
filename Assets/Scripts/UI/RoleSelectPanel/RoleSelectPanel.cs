/*******************************************************/
/**2020-7-2 16:19****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Sound;
using GameFrameWork.Input;

public class RoleSelectPanel : BasePanel
{
	public override string panelName { get { return "RoleSelectPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Eternal; } }

    protected override void OnInit(object[] param)
	{
		m_Component = new RoleSelectPanelComponent(uiRefRoot);
		m_Component.roleContentGroupView.Init(m_Component.roleContent, m_Component.itemGO, 3);

	}

    protected override void OnOpen()
    {
		m_HasSelect = false;

		m_Component.imgSelectRect.gameObject.SetActive(true);
		m_Component.roleContentGroupView.onItemUpdateEvent = OnItemUpdate;
		m_Component.roleContentGroupView.onItemSelectEvent = OnItemSelect;

		m_Component.roleContentGroupView.Update(DataHelper.roleSelectDatas.Length);
		m_Component.roleContentGroupView.SelectItem(0);

		SoundMgr.instance.PlayBGM(ResDefine.AudioClipPath, "BGM/bgm14Character", true);
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
				if (m_CurrSelectIndex >= DataHelper.roleSelectDatas.Length) m_CurrSelectIndex = 0;
			}
			else
			{
				m_CurrSelectIndex--;
				if (m_CurrSelectIndex < 0) m_CurrSelectIndex = DataHelper.roleSelectDatas.Length - 1;
			}

           m_Component.roleContentGroupView.SelectItem(m_CurrSelectIndex);
            SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnSelect");
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
		RoleSelectData roleSelectData = DataHelper.roleSelectDatas[item.index];

		item.txtDesc.text = roleSelectData.desc;
		item.txtName.text = roleSelectData.name;
		item.btnRoleIcon.image.SetSprite(roleSelectData.headIcon);
	}

	private void OnItemSelect(RoleSelectPanelComponent.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			m_Component.imgSelectRect.SetParent(item.btnRoleIcon.transform, false);
			m_Component.imgSelectRect.localPosition = Vector3.zero;
			m_CurrSelectIndex = item.index;
		}
	}

	private void EnterStage()
	{
        m_Component.imgSelectRect.GetComponent<UIFrameEffect>().StopFrame();

        SoundMgr.instance.StopBGM();
		SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/OnSelected");
		PlayerMgr.instance.selectRoleId = DataHelper.roleSelectDatas[m_CurrSelectIndex].roleId;
		StageMgr.instance.nextStageId = StaticConfig.StageConfig.GetDataByIndex(0).Id;
		UIMgr.instance.Open<LoadPanel>().DOFade(0f, 1f, 0.3f, 0.5f, () =>
		{
			UIMgr.instance.Close<LoadPanel>();
			UIMgr.instance.Open<StagePanel>();
			InnerClose();
		});
	}

	private bool m_HasSelect = false;
	private int m_CurrSelectIndex = -1;
	private RoleSelectPanelComponent m_Component = null;
}