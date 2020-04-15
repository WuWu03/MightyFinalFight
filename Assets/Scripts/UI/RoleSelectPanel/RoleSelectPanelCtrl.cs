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

public class RoleSelectPanelCtrl : BasePanelCtrl
{
	private RoleSelectPanel panel = null;
	protected override void OnInit(object[] param)
	{
		panel = Panel as RoleSelectPanel;
	}

	protected override void OnLoaded()
	{
		panel.RoleContentGroupView.Init(panel.RoleContent, panel.ItemGO, 3);
	}
	protected override void OnOpen()
	{
		panel.ImgSelectRect.gameObject.SetActive(true);
		panel.RoleContentGroupView.OnItemUpdate = OnItemUpdate;
		panel.RoleContentGroupView.OnItemSelect = OnItemSelect;

		panel.RoleContentGroupView.Update(1);
		panel.RoleContentGroupView.SelectItem(0);
	}

	protected override void OnUpdate()
	{
		if (Input.GetButtonDown("A") || Input.GetButton("X"))
		{
			InnerClose();
			PlayerMgr.Ins.InitPlayer(1001);
			CameraMgr.Ins.SetTarget(PlayerMgr.Ins.Player.transform);
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
		UITools.SetIconSprite(ResDefine.ICON_PATH + "/Character/Cody", item.BtnRoleIcon.image);
	}

	private void OnItemSelect(RoleSelectPanel.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			panel.ImgSelectRect.SetParent(item.BtnRoleIcon.transform, false);
			panel.ImgSelectRect.localPosition = Vector3.zero;
		}
	}
}