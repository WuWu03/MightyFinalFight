
//===================================================
//作者：GQY                                          
//==创建时间：2020-02-01 16:39:33
//备注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using LitJson;

/// <summary>
/// Role数据表
/// </summary>
namespace LocalData
{
	public partial class RoleData : AbstractData
	{
		/// <summary>
		/// 角色名字
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 角色描述
		/// </summary>
		public string Desc { get; set; }

		/// <summary>
		/// 角色模型
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// 角色头像
		/// </summary>
		public string Icon { get; set; }

		public RoleData Clone()
		{
			RoleData roleData = new RoleData();
			roleData.Name = this.Name;
			roleData.Desc = this.Desc;
			roleData.Model = this.Model;
			roleData.Icon = this.Icon;
			return roleData;
		}

		internal override void Read(GameDataTableParser parser)
		{
			this.ID = parser.GetFieldValue("ID").ToInt();
			this.Name = parser.GetFieldValue("Name");
			this.Desc = parser.GetFieldValue("Desc");
			this.Model = parser.GetFieldValue("Model");
			this.Icon = parser.GetFieldValue("Icon");
		}
	}
}
