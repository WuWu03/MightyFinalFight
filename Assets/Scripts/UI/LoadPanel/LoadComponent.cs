/*******************************************************/
/**2025-07-16 19:34*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/

using UnityEngine.UI;
using GameFrameWork.UI;

public class LoadComponent : UIBaseComponent
{
	//imgShade,Image
	public Image imgShade { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		imgShade = root.objects[0] as Image;
	}
}