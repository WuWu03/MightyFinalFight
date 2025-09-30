/*******************************************************/
/**2025-08-08 14:43*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/

using GameFrameWork.UI;

public class VersionComponent : UIBaseComponent
{
	//txtVersion,LanguageText
	public LanguageText txtVersion { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		txtVersion = root.objects[0] as LanguageText;
	}
}