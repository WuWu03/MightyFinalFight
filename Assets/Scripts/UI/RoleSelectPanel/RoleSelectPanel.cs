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
		SoundMgr.Ins.PlayBGM(ResDefine.AUDIO_CLIP_PATH + "/BGM", "bgm14Character", true);
		m_Component.ImgSelectRect.gameObject.SetActive(true);
		m_Component.RoleContentGroupView.OnItemUpdate = OnItemUpdate;
		m_Component.RoleContentGroupView.OnItemSelect = OnItemSelect;

		m_Component.RoleContentGroupView.Update(StaticConfig.RoleSelectConfig.Datas.Count);
		m_Component.RoleContentGroupView.SelectItem(0);
	}

    protected override void OnUpdate()
    {
		Vector2 axis = InputMgr.GetAxis(true);
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

			SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnSelect");
			m_Component.RoleContentGroupView.SelectItem(m_CurrSelectIndex);
		}

		if (m_CurrSelectIndex != -1 && (Input.GetButtonDown("A") || Input.GetButton("X")))
		{
			SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/OnSelected");
			PlayerMgr.Ins.SelectId = StaticConfig.RoleSelectConfig.Datas[m_CurrSelectIndex].CharacterId;
			StageMgr.Ins.Enter(StaticConfig.StageConfig.GetDataByIndex(2).Id);
			InnerClose();
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

	private int m_CurrSelectIndex = -1;
	private RoleSelectPanelComponent m_Component = null;
}