/*******************************************************/
/**2025-08-16 14:22*************************************/
/**Create By WuWu***************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/

using UnityEngine;
using GameFrameWork.UI;

public class StageComponent : UIBaseComponent
{
	//blue,GameObject
	public GameObject blue { get; private set; }
	//red,GameObject
	public GameObject red { get; private set; }
	//green,GameObject
	public GameObject green { get; private set; }
	//imgMap,GameObject
	public GameObject imgMapGO { get; private set; }
	//heroPos,GameObject
	public GameObject heroPosGO { get; private set; }

	protected override void OnInitComponent(UIRefRoot root)
	{
		blue = root.objects[0] as GameObject;
		red = root.objects[1] as GameObject;
		green = root.objects[2] as GameObject;
		imgMapGO = root.objects[3] as GameObject;
		heroPosGO = root.objects[4] as GameObject;
	}
}