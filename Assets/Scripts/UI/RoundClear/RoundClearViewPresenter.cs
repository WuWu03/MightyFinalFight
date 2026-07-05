/*
 * @Desc: RoundClear 模块 RoundClearView 视图展示器
 * @Date: 2023-11-29 19:31:08
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using WuWuFramework.UI;

public class RoundClearViewPresenter : UIBaseViewPresenter<RoundClearView>
{
	protected override void OnOpen(object arg)
	{

	}

	protected override void OnShow(object arg)
	{
		int stageLevel = StageMgr.instance.currStageData.StageIndex;
        view.txtRound.SetLanguageTextParams(stageLevel.ToString());
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