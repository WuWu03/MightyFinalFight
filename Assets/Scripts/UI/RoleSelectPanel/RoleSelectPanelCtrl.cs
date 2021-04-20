/*******************************************************/
/**2020-4-4 17:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;
using GameFrameWork.Sound;
using GameFrameWork.Input;

public class RoleSelectPanelCtrl : BasePanelCtrl
{
	protected override void OnInit(object[] param)
	{
		m_Panel = Panel as RoleSelectPanel;
	}

	protected override void OnLoaded()
	{

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

	protected override BasePanel GetPanel()
	{		
		return new RoleSelectPanel();
	}


	private RoleSelectPanel m_Panel = null;
}