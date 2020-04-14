/*******************************************************/
/**2020-4-14 20:34**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
public class MainPanelCtrl:BasePanelCtrl
{
	protected override void OnInit(object[] param)
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

	
}