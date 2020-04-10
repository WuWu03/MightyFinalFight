///*******************************************************
//**2020-2-2 16:44**************************************
//**Create By GQY*****************************************
//*******************************************************/
//using UnityEngine;
//using UnityEngine.UI;
//using FrameWork.UI;
//using FrameWork.Input;

//namespace Runtime
//{
//	public class RoleSelectPanel : BasePanel
//	{
//		protected override string PanelName
//		{
//			get { return "RoleSelectPanel"; }
//		}

//		public Image ImgSelect
//		{
//			get;
//			private set;
//		}

//		public override UIMgr.UILayer PanelLayer
//		{
//			get
//			{
//				return UIMgr.UILayer.FirstLevel;
//			}
//		}

//		protected override void OnInit()
//		{
//			m_RoleListView = new LayoutGroupView<RoleSelectItem, RoleSelectPanel>();
//		}

//		protected override void OnLoadViewCallback()
//		{
//			m_Roles = transform.Find("_roles").gameObject;
//			ImgSelect = transform.Find("_roles/_imgSelect").GetComponent<Image>();
//			ImgSelect.gameObject.SetActive(true);
//			m_RoleListView.Init(this, m_Roles, 1);
//			m_RoleListView.Update(3);
//			ImgSelect.transform.SetAsLastSibling();
//		}

//		protected override void OnAfterOpenHandle()
//		{
//			m_RoleListView.SelectItem(0);
//		}

//		protected override void OnBeforeCloseHandle()
//		{

//		}

//		protected override void OnUpdate()
//		{
//			Vector2 axis = InputMgr.GetAxis();
//			if (axis.y == 0)
//			{
//				m_CanSelect = true;
//			}

//			if (m_CanSelect && axis.y != 0)
//			{
//				SelectItem(axis.y);
//				m_CanSelect = false;
//			}
//		}

//		protected override void OnClose()
//		{
//		}

//		protected override void OnDestroy()
//		{

//		}

//		private void SelectItem(float dir)
//		{
//			if (dir < 0) m_RoleIndex++;
//			else m_RoleIndex--;

//			if (m_RoleIndex > 2) m_RoleIndex = 0;
//			if (m_RoleIndex < 0) m_RoleIndex = 2;

//			m_RoleListView.SelectItem(m_RoleIndex);
//		}

//		private bool m_CanSelect = false;
//		private int m_RoleIndex = 0;
//		private LayoutGroupView<RoleSelectItem, RoleSelectPanel> m_RoleListView = null;
//		private GameObject m_Roles = null;

//		class RoleSelectItem : LayoutViewItem<RoleSelectPanel>
//		{
//			public override void CreateHandle(RoleSelectPanel panel)
//			{
//				base.CreateHandle(panel);
//				m_BtnRoleIcon = transform.Find("_btnRoleIcon").GetComponent<MyButton>();
//				m_TxTName = transform.Find("_txtName").GetComponent<Text>();
//				m_TxTDesc = transform.Find("_txtDesc").GetComponent<Text>();
//				m_BtnRoleIcon.onClick.AddListener(onClick);
//			}

//			public override void SetData(int index)
//			{
//				RoleData roleData = DataHelper.RoleData[0];
//				UIMgr.Ins.SetIconSprite(roleData.Icon, m_BtnRoleIcon.GetComponent<Image>());
//				m_TxTName.text = roleData.Name;
//				m_TxTDesc.text = roleData.Desc;
//			}

//			public override void SelectHandle(bool isSelect)
//			{
//				if (isSelect)
//				{
//					Vector3 selectPos = m_BtnRoleIcon.transform.position;
//					Panel.ImgSelect.transform.position = selectPos;
//					m_BtnRoleIcon.Select();
//				}
//			}

//			private void onClick()
//			{
//				Debug.Log("Click");
//			}

//			private MyButton m_BtnRoleIcon = null;
//			private Text m_TxTName = null;
//			private Text m_TxTDesc = null;
//		}
//	}
//}