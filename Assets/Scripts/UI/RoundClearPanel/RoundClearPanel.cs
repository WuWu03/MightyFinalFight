/*******************************************************/
/**2023-11-29 19:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;

public class RoundClearPanel : BasePanel
{
	public override string panelName { get { return "RoundClearPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new RoundClearPanelComponent(m_UIRefRoot);
	}

	protected override void OnOpen()
	{
		m_Component.txtRound.text = StageMgr.instance.currStageData.StageIndex.ToString();
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

	private RoundClearPanelComponent m_Component = null;
}