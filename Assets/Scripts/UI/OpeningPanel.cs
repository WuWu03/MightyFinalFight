/*******************************************************/
/**2021-9-6 21:9****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Sound;

public class OpeningPanel : BasePanel
{
	public override string PanelName { get { return "OpeningPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Destroy; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new OpeningPanelComponent(UIRefRoot);
	}

	protected override void OnOpen()
	{
		SoundMgr.Ins.PlayBGM(ResDefine.AUDIO_CLIP_PATH, "BGM/bgm13Title", true);
	}

	protected override void OnUpdate()
	{
		if (Input.GetButtonDown("Start"))
		{
			UIMgr.Ins.Open<RoleSelectPanel>();
		}
	}

	protected override void OnClose()
	{
	}

	protected override void OnDestroy()
	{
	}

	private OpeningPanelComponent m_Component = null;
}