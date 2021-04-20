/*******************************************************/
/**2020-4-14 20:34**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
using DG.Tweening;
using System;
using System.Security.Principal;

public class MainPanelCtrl:BasePanelCtrl
{
	protected override void OnInit(object[] param)
	{
		m_Panel = Panel as MainPanel;
	}

	protected override void OnLoaded()
	{
		
	}

	protected override BasePanel GetPanel()
	{
		return new MainPanel();
	}

	protected override void OnOpen()
	{
		
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



	
	private MainPanel m_Panel = null;
}