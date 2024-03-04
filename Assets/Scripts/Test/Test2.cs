using GameFrameWork.Utilities;
using System.Drawing;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Profiling;

public class Test2 : MonoBehaviour
{
    class A
    {
        public int b;
    }
    private void Awake()
    {
        string a = StringUtil.GetRomanValue(12345);
        Debug.Log(a);
    }

    private void Update()
    {
        Profiler.BeginSample("new AAA");
        A a = new A();
        //Debug.Log(a);
        Profiler.EndSample();
    }
}