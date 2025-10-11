/*
 * @Desc: RoundClear 模块 RoundClearView 界面数据
 * @Date: 2023-11-29 19:31:08
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using GameFrameWork.UI;

public class RoundClearView : UIBaseView<RoundClearViewComponent, RoundClearViewSettings>
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