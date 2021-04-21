#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class UIRef : MonoBehaviour
{
    /// <summary>
    /// 使用GameObject的名字
    /// </summary>
    [SerializeField] private bool m_UseObjName;
    public bool UseObjName
    {
        get { return m_UseObjName; }
        set { m_UseObjName = value;}
    }

    /// <summary>
    /// 字段名称
    /// </summary>
    [SerializeField] private string m_RefName;
    public string Name
    {
        get { return m_RefName; }
        set { m_RefName = value; }
    }
    /// <summary>
    /// 引用组件的名称
    /// </summary>
    [SerializeField] private string m_ComponentName;
    public string ComponentName 
    { 
        get { return m_ComponentName; }
        set { m_ComponentName = value; }
    }

    /// <summary>
    /// 描述
    /// </summary>
    [SerializeField] private string m_Desc;
    public string Desc 
    {
        get { return m_Desc; }
        set { m_Desc = value; }
    }

    /// <summary>
    /// 列表格子
    /// </summary>
    [SerializeField] private bool m_IsLayoutItem;
    public bool IsLayoutItem
    {
        get { return m_IsLayoutItem; }
        set { m_IsLayoutItem = value; }
    }

    /// <summary>
    /// 列表格子
    /// </summary>
    [SerializeField] private bool m_IsLayoutItemVariable;
    public bool IsLayoutItemVariable
    {
        get { return m_IsLayoutItemVariable; }
        set { m_IsLayoutItemVariable = value; }
    }

    /// <summary>
    /// 循环列表
    /// </summary>
    [SerializeField] private bool m_IsLoopLayout;
    public bool IsLoopLayout
    {
        get { return m_IsLoopLayout; }
        set { m_IsLoopLayout = value; }
    }
    /// <summary>
    /// 输出到剪切板
    /// </summary>
    [SerializeField] private bool m_IsCopyRefStr;
    public bool IsCopyRefStr
    {
        get { return m_IsCopyRefStr; }
        set { m_IsCopyRefStr = value; }
    }
}
#endif