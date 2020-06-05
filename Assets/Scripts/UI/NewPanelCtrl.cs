/*******************************************************/
/**2020-5-22 11:42**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using System;

public class NewPanelCtrl:BasePanelCtrl
{
	protected override void OnInit(object[] param)
	{
		m_Panel = Panel as NewPanel;
	}

	protected override void OnLoaded()
	{
	}

	protected override void OnOpen()
	{
		m_Panel.List1GroupView.OnItemUpdate = OnUpdateItem;
	}

	private void OnUpdateItem(NewPanel.List1Item obj)
	{
		//obj.Icon.sprite = 
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
		return new NewPanel();
	}

	private NewPanel m_Panel = null;
}