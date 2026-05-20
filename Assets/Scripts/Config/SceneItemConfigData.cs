
//===================================================
//作者：GQY                                          
//创建时间：2024-06-06 11:09:24
//备注：此代码为工具生成 请勿手工修改
//===================================================
using GameFrameWork;
using GameFrameWork.ConfigData;

/// <summary>
/// SceneItem.xlsx数据表
/// SheetName:Sheet1
/// </summary>
public class SceneItemConfigData : BaseConfigData
{
	/// <summary>
	/// 名字
	/// </summary>
	public string name { get; private set; }

	/// <summary>
	/// 资源
	/// </summary>
	public string assetName { get; private set; }

	/// <summary>
	/// 类型
	/// </summary>
	public int type { get; private set; }

	/// <summary>
	/// 生命或经验
	/// </summary>
	public int value { get; private set; }

	/// <summary>
	/// 武器是否可以掉落
	/// </summary>
	public bool canDrop { get; private set; }

	public SceneItemConfigData Clone()
	{
		SceneItemConfigData sceneItemConfigData = new SceneItemConfigData();
		sceneItemConfigData.name = this.name;
		sceneItemConfigData.assetName = this.assetName;
		sceneItemConfigData.type = this.type;
		sceneItemConfigData.value = this.value;
		sceneItemConfigData.canDrop = this.canDrop;
		return sceneItemConfigData;
	}

	public override void Read(ConfigDataParser parser)
	{
		this.id = parser.GetFieldValue("id").ToInt();
		this.name = parser.GetFieldValue("name");
		this.assetName = parser.GetFieldValue("assetName");
		this.type = parser.GetFieldValue("type").ToInt();
		this.value = parser.GetFieldValue("value").ToInt();
		this.canDrop = parser.GetFieldValue("canDrop").ToBool();
	}
}
