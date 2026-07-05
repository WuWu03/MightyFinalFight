/*
 * @Desc: UI工厂
 * @Date: 2026-07-04 19:22:42
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

namespace WuWuFramework.UI
{
	public static partial class UIFactory
	{
		static UIFactory()
		{
			s_Factories.Add(typeof(HudView), CreateUIView<HudView>);
			s_Factories.Add(typeof(LoadView), CreateUIView<LoadView>);
			s_Factories.Add(typeof(MainView), CreateUIView<MainView>);
			s_Factories.Add(typeof(RoleSelectView), CreateUIView<RoleSelectView>);
			s_Factories.Add(typeof(RoundClearView), CreateUIView<RoundClearView>);
			s_Factories.Add(typeof(StageView), CreateUIView<StageView>);
			s_Factories.Add(typeof(TalkView), CreateUIView<TalkView>);
			s_Factories.Add(typeof(TitleView), CreateUIView<TitleView>);
			s_Factories.Add(typeof(VersionView), CreateUIView<VersionView>);
		}
	}
}