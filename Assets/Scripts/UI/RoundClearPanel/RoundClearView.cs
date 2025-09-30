/*******************************************************/
/**2023-11-29 19:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using GameFrameWork.UI;

public class RoundClearView : UIBaseView<RoundClearComponent, RoundClearSettings>
{
	protected override void OnOpen(object arg)
	{

	}

	protected override void OnShow(object arg)
	{
		component.txtRound.SetLanguageTextParams("1");// StageMgr.instance.currStageData.StageIndex.ToString());
	}

	protected override void OnUpdate()
	{
        
	}

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
	{
	}

	protected override void OnDestroy()
	{

	}
}