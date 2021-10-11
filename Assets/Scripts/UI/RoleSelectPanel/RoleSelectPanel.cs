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
	public override string PanelName { get { return "RoleSelectPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Eternal; } }

    protected override void OnInit(object[] param)
	{
		m_Component = new RoleSelectPanelComponent(UIRefRoot);
		m_Component.RoleContentGroupView.Init(m_Component.RoleContent, m_Component.ItemGO, 3);

	}

    protected override void OnOpen()
    {
		m_HasSelect = false;
		SoundMgr.Ins.PlayBGM(ResDefine.AudioClipPath, "BGM/bgm14Character", true);
		m_Component.ImgSelectRect.gameObject.SetActive(true);
		m_Component.RoleContentGroupView.OnItemUpdate = OnItemUpdate;
		m_Component.RoleContentGroupView.OnItemSelect = OnItemSelect;

		m_Component.RoleContentGroupView.Update(StaticConfig.RoleSelectConfig.Datas.Count);
		m_Component.RoleContentGroupView.SelectItem(0);
	}

    protected override void OnUpdate()
    {
		if(m_HasSelect)
        {
			return;
        }

		Vector2 axis = InputMgr.Ins.GetAxis(AxisType.LeftAxis, true);

		if (axis.y != 0)
		{
			if (axis.y < 0)
			{
				m_CurrSelectIndex++;
				if (m_CurrSelectIndex >= StaticConfig.RoleSelectConfig.Datas.Count) m_CurrSelectIndex = 0;
			}
			else
			{
				m_CurrSelectIndex--;
				if (m_CurrSelectIndex < 0) m_CurrSelectIndex = StaticConfig.RoleSelectConfig.Datas.Count - 1;
			}

			SoundMgr.Ins.PlaySound(ResDefine.AudioClipPath, "Sound/OnSelect");
			m_Component.RoleContentGroupView.SelectItem(m_CurrSelectIndex);
		}

		if (m_CurrSelectIndex != -1 && (InputMgr.Ins.GetKeyDown(KeyType.A, true) || InputMgr.Ins.GetKeyDown(KeyType.X, true)))
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
		int roleId = StaticConfig.RoleSelectConfig.Datas[item.Index].CharacterId;
		CharacterConfigData data = StaticConfig.CharacterConfig.GetData(roleId);
		item.TxtDesc.text = data.Desc;
		item.TxtName.text = data.Name;
		item.BtnRoleIcon.image.SetSprite(data.HeadIcon);
	}

	private void OnItemSelect(RoleSelectPanelComponent.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			m_Component.ImgSelectRect.SetParent(item.BtnRoleIcon.transform, false);
			m_Component.ImgSelectRect.localPosition = Vector3.zero;
			m_CurrSelectIndex = item.Index;
		}
	}

	private void EnterStage()
	{
		SoundMgr.Ins.StopBGM();
		SoundMgr.Ins.PlaySound(ResDefine.AudioClipPath, "Sound/OnSelected");
		PlayerMgr.Ins.SelectCharacterId = StaticConfig.RoleSelectConfig.Datas[m_CurrSelectIndex].CharacterId;
		StageMgr.Ins.NextStageId = StaticConfig.StageConfig.GetDataByIndex(0).Id;
		m_Component.ImgSelectRect.GetComponent<UIFrameEffect>().StopFrame();

		UIMgr.Ins.Open<LoadPanel>().DOFade(0f, 1f, 0.3f, 0.5f, () =>
		{
			UIMgr.Ins.Close<LoadPanel>();
			UIMgr.Ins.Open<StagePanel>();
			InnerClose();
		});
	}

	private bool m_HasSelect = false;
	private int m_CurrSelectIndex = -1;
	private RoleSelectPanelComponent m_Component = null;
}