/*******************************************************/
/**2023-11-29 19:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using GameFrameWork.UI;

public class RoundClearPanel : BasePanel<RoundClearPanelComponent>
{
    protected override void OnInit(object[] param)
    {

    }

	protected override void OnOpen()
	{
		m_Component.txtRound.SetLanguageTextParams("1");// StageMgr.instance.currStageData.StageIndex.ToString());
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