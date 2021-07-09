#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class UIRef : MonoBehaviour
{
    /// <summary>
    /// 使用GameObject的名字
    /// </summary>
    public bool UseDefaultName
    {
        get 
        {
            return m_UseDefaultName; 
        }
        set 
        {
            m_UseDefaultName = value;
        }
    }

    /// <summary>
    /// 字段名称
    /// </summary>

    public string Name
    {
        get 
        { 
            return m_RefName; 
        }
        set 
        {
            m_RefName = value; 
        }
    }

    /// <summary>
    /// 引用组件的名称
    /// </summary>
    public string ComponentName 
    { 
        get 
        {
            return m_ComponentName; 
        }
        set 
        {
            m_ComponentName = value; 
        }
    }

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc 
    {
        get 
        { 
            return m_Desc; 
        }
        set 
        { 
            m_Desc = value; 
        }
    }

    /// <summary>
    /// 列表格子
    /// </summary>
    public bool IsLayoutItem
    {
        get 
        {
            return m_IsLayoutItem; 
        }
        set
        {
            m_IsLayoutItem = value;
        }
    }

    /// <summary>
    /// 列表格子成员
    /// </summary>
    public bool IsLayoutItemVariable
    {
        get
        {
            return m_IsLayoutItemVariable;
        }
        set
        {
            m_IsLayoutItemVariable = value;
        }
    }

    /// <summary>
    /// 循环列表
    /// </summary>
    public bool IsLoopLayout
    {
        get
        {
            return m_IsLoopLayout;
        }
        set
        {
            m_IsLoopLayout = value;
        }
    }

    /// <summary>
    /// 输出到剪切板
    /// </summary>
    public bool IsCopyRefStr
    {
        get 
        {
            return m_IsCopyRefStr;
        }
        set
        {
            m_IsCopyRefStr = value;
        }
    }

    [SerializeField] private bool m_UseDefaultName;
    [SerializeField] private string m_RefName;
    [SerializeField] private string m_ComponentName;
    [SerializeField] private string m_Desc;
    [SerializeField] private bool m_IsLayoutItem;
    [SerializeField] private bool m_IsLayoutItemVariable;
    [SerializeField] private bool m_IsLoopLayout;
    [SerializeField] private bool m_IsCopyRefStr;
}
#endif