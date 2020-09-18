/*******************************************************/
/**2020-4-4 17:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using FrameWork.Sound;
using FrameWork.Input;

public class RoleSelectPanelCtrl : BasePanelCtrl
{
	protected override void OnInit(object[] param)
	{
		m_Panel = Panel as RoleSelectPanel;
	}

	protected override void OnLoaded()
	{
		m_Panel.RoleContentGroupView.Init(m_Panel.RoleContent, m_Panel.ItemGO, 3);
	}
	protected override void OnOpen()
	{
		SoundMgr.Ins.PlayBGM(ResDefine.AUDIO_CLIP_PATH + "/BGM", "bgm14Character", true);
		m_Panel.ImgSelectRect.gameObject.SetActive(true);
		m_Panel.RoleContentGroupView.OnItemUpdate = OnItemUpdate;
		m_Panel.RoleContentGroupView.OnItemSelect = OnItemSelect;

		m_Panel.RoleContentGroupView.Update(StaticConfig.HeroConfig.Datas.Length);
		m_Panel.RoleContentGroupView.SelectItem(0);
	}

	protected override void OnUpdate()
	{
		Vector2 axis = InputMgr.GetAxis(true);
		if (axis.y != 0)
		{
			if (axis.y > 0)
			{
				m_CurrSelectIndex++;
				if (m_CurrSelectIndex >= StaticConfig.HeroConfig.Datas.Length) m_CurrSelectIndex = 0;
			}
			else
			{
				m_CurrSelectIndex--;
				if (m_CurrSelectIndex < 0) m_CurrSelectIndex = StaticConfig.HeroConfig.Datas.Length - 1;
			}

			SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnSelect");
			m_Panel.RoleContentGroupView.SelectItem(m_CurrSelectIndex);
		}

		if (m_CurrSelectIndex != -1 && (Input.GetButtonDown("A") || Input.GetButton("X")))
		{
			SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnSelected");
			InnerClose();
			PlayerMgr.Ins.InitPlayer(StaticConfig.HeroConfig.Datas[m_CurrSelectIndex].ID);
			StageMgr.Ins.Enter(1001);
		}
	}

	protected override void OnClose()
	{

	}

	protected override void OnDestroy()
	{
	}

	protected override BasePanel GetPanel()
	{		
		return new RoleSelectPanel();
	}

	private void OnItemUpdate(RoleSelectPanel.RoleContentItem item)
	{
		HeroData data = StaticConfig.HeroConfig.Datas[item.Index - 1];
		item.TxtDesc.text = data.Desc;
		item.TxtName.text = data.Name;
		UITools.LoadSprite("Character", data.HeadIcon, item.BtnRoleIcon.image);
	}

	private void OnItemSelect(RoleSelectPanel.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			m_Panel.ImgSelectRect.SetParent(item.BtnRoleIcon.transform, false);
			m_Panel.ImgSelectRect.localPosition = Vector3.zero;
			m_CurrSelectIndex = item.Index - 1;
		}
	}

	private int m_CurrSelectIndex = -1;
	private RoleSelectPanel m_Panel = null;
}