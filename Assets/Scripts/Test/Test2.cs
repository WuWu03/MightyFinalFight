using GameFrameWork.Utilities;
using System.Drawing;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    private void Awake()
    {
        string a = StringUtil.GetRomanValue(12345);
        Debug.Log(a);
    }
}