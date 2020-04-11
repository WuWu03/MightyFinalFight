/*******************************************************/
/**2020-4-4 17:31****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
public class RoleSelectPanelCtrl : BasePanelCtrl<RoleSelectPanel,RoleSelectPanelCtrl>
{
	protected override void OnInit()
	{
		m_RoleListView = new LayoutGroupView<RoleSelectItem, RoleSelectPanel>();
	}

	protected override void OnOpen()
	{
		Panel.TxtRoleName.text = "sdfsdf111";
		Panel.ImgSelectGO.SetActive(false);
	}

	protected override void OnUpdate()
	{
	}

	protected override void OnClose()
	{
	}

	protected override void OnDestroy()
	{
	}

	class RoleSelectItem : LayoutViewItem<RoleSelectPanel>
	{
		public override void CreateHandle(RoleSelectPanel panel)
		{
			base.CreateHandle(panel);
			m_BtnRoleIcon = transform.Find("_btnRoleIcon").GetComponent<MyButton>();
			m_TxTName = transform.Find("_txtName").GetComponent<Text>();
			m_TxTDesc = transform.Find("_txtDesc").GetComponent<Text>();
			//m_BtnRoleIcon.onClick.AddListener(onClick);
		}

		public override void SetData(int index)
		{
			//RoleData roleData = DataHelper.RoleData[0];
			//UIMgr.Ins.SetIconSprite(roleData.Icon, m_BtnRoleIcon.GetComponent<Image>());
			//m_TxTName.text = roleData.Name;
			//m_TxTDesc.text = roleData.Desc;
		}

		public override void SelectHandle(bool isSelect)
		{
			if (isSelect)
			{
				Vector3 selectPos = m_BtnRoleIcon.transform.position;
				//Panel.ImgSelect.transform.position = selectPos;
				m_BtnRoleIcon.Select();
			}
		}

		private MyButton m_BtnRoleIcon = null;
		private Text m_TxTName = null;
		private Text m_TxTDesc = null;
	}

	private LayoutGroupView<RoleSelectItem, RoleSelectPanel> m_RoleListView = null;
}