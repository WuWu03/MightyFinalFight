/*******************************************************/
/**2023-11-29 19:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using GameFrameWork.UI;

public class RoundClearPanel : BasePanel
{
    protected override void OnInit(object[] param)
    {
        m_Component = GetPanelComponent<RoundClearPanelComponent>();
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