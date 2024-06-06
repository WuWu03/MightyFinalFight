/*******************************************************/
/**2023-11-29 19:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using GameFrameWork.UI;
using System;

public class RoundClearPanel : BasePanel
{
    protected override Type componentType
    {
        get
        {
            return typeof(RoundClearPanelComponent);
        }
    }

    protected override Type settingsType
    {
        get
        {
            return typeof(RoundClearPanelSettings);
        }
    }

    protected override void OnInit(BasePanelComponent panelComponent, object[] param)
    {
        m_Component = panelComponent as RoundClearPanelComponent;
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