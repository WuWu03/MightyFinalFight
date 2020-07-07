/*******************************************************/
/**2020-4-4 17:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using System;
using FrameWork.Camera;
using FrameWork.Sound;

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

		m_Panel.RoleContentGroupView.Update(1);
		m_Panel.RoleContentGroupView.SelectItem(0);
	}

	protected override void OnUpdate()
	{
		if (Input.GetButtonDown("A") || Input.GetButton("X"))
		{
			FrameWork.Sound.SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH + "/Sound", "OnSelected");
			InnerClose();
			PlayerMgr.Ins.InitPlayer(1001);
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
		HeroData data = StaticConfig.HeroConfig.GetData(1001);
		item.TxtDesc.text = data.Desc;
		item.TxtName.text = data.Name;
		UITools.LoadSprite("Character", "Cody", item.BtnRoleIcon.image);
	}

	private void OnItemSelect(RoleSelectPanel.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			m_Panel.ImgSelectRect.SetParent(item.BtnRoleIcon.transform, false);
			m_Panel.ImgSelectRect.localPosition = Vector3.zero;
		}
	}

	private RoleSelectPanel m_Panel = null;
}