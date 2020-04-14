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

public class RoleSelectPanelCtrl : BasePanelCtrl
{
	private RoleSelectPanel panel = null;
	protected override void OnInit(object[] param)
	{
		panel = Panel as RoleSelectPanel;
	}

	protected override void OnOpen()
	{
		panel.ImgSelectRect.gameObject.SetActive(true);
		panel.RoleContentGroupView.OnItemUpdate = OnItemUpdate;
		panel.RoleContentGroupView.OnItemSelect = OnItemSelect;
		panel.RoleContentGroupView.Init(panel.RoleContent, panel.ItemGO, 3);
		panel.RoleContentGroupView.Update(1);
		panel.RoleContentGroupView.SelectItem(1);
	}

	protected override void OnUpdate()
	{

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
		Runtime.Config.HeroData data = Runtime.StaticConfig.HeroConfig.GetData(1001);
		item.TxtDesc.text = data.Desc;
		item.TxtName.text = data.Name;
		UITools.SetIconSprite("Character/Cody", item.BtnRoleIcon.image);
	}

	private void OnItemSelect(RoleSelectPanel.RoleContentItem item, bool isSelect)
	{
		if (isSelect)
		{
			panel.ImgSelectRect.SetParent(item.transform, false);
			panel.ImgSelectRect.localPosition = item.transform.localPosition;
		}
	}
}