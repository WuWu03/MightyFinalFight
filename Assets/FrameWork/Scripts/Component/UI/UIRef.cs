#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class UIRef : MonoBehaviour
{
    /// <summary>
    /// 使用GameObject的名字
    /// </summary>
    public bool UseObjName { get; set; }

    /// <summary>
    /// 字段名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 引用组件的名称
    /// </summary>
    public string ComponentName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; set; }

    /// <summary>
    /// 输出到剪切板
    /// </summary>
    public bool OutputClipBoard { get; set; }
}
#endif