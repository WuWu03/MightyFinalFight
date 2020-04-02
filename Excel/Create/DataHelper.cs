
//===================================================
//作者：GQY                                          
//==创建时间：2020-02-01 16:39:33
//备注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;

/// <summary>
///数据总表
/// </summary>
namespace LocalData
{
	public static partial class DataHelper
	{
		public static RoleData[] RoleData;

		public static void Init()
		{
			RoleData = LoadData<RoleData>("Role.data");
		}
	}
}