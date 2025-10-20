using UnityEngine;

#if UNITY_EDITOR
namespace GameFrameWork.UI
{
    public class UIRef : MonoBehaviour
    {
        /// <summary>
        /// 使用GameObject的名字
        /// </summary>
        public bool useDefaultName
        {
            get { return m_UseDefaultName; }
            set { m_UseDefaultName = value; }
        }

        /// <summary>
        /// 字段名称
        /// </summary>

        public string refName
        {
            get { return m_RefName; }
            set { m_RefName = value; }
        }

        /// <summary>
        /// 引用组件的名称
        /// </summary>
        public string componentName
        {
            get { return m_ComponentName; }
            set { m_ComponentName = value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string desc
        {
            get { return m_Desc; }
            set { m_Desc = value; }
        }

        /// <summary>
        /// 列表格子
        /// </summary>
        public bool isListItem
        {
            get { return m_IsListItem; }
            set { m_IsListItem = value; }
        }

        /// <summary>
        /// 列表格子成员
        /// </summary>
        public bool IsListItemVariable
        {
            get { return m_IsListItemVariable; }
            set { m_IsListItemVariable = value; }
        }

        public bool IsList
        {
            get { return m_IsList; }
            set { m_IsList = value; }
        }

        /// <summary>
        /// 输出到剪切板
        /// </summary>
        public bool isCopyRefStr
        {
            get { return m_IsCopyRefStr; }
            set { m_IsCopyRefStr = value; }
        }

        [SerializeField] private bool m_UseDefaultName;
        [SerializeField] private string m_RefName;
        [SerializeField] private string m_ComponentName;
        [SerializeField] private string m_Desc;
        [SerializeField] private bool m_IsListItem;
        [SerializeField] private bool m_IsListItemVariable;
        [SerializeField] private bool m_IsList;
        [SerializeField] private bool m_IsCopyRefStr;
    }
}
#endif