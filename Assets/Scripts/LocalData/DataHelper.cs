
//===================================================
//作者：GQY                                          
//创建时间：2022-09-07 16:36:31
//备注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using GameFrameWork.LocalData;

/// <summary>
///数据总表
/// </summary>
public static partial class DataHelper
{
	public static LevelData[] levelDatas = null;
	public static RoleData[] roleDatas = null;
	public static RoleSelectData[] roleSelectDatas = null;

	public static void Init(string filePath)
	{
        levelDatas = LoadData<LevelData>(filePath, "LevelData");
        roleDatas = LoadData<RoleData>(filePath, "RoleData");
        roleSelectDatas = LoadData<RoleSelectData>(filePath, "RoleSelectData");
	}

}
