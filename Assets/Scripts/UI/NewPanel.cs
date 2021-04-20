/*******************************************************/
/**2021-4-19 14:30****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;

public class NewPanel : BasePanel
{
	public override string PanelName { get { return "NewPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new NewPanelComponent(UIRefRoot);
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

	private NewPanelComponent m_Component = null;
}