using GameFrameWork;
using GameFrameWork.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    public GuideMaskImage mask;
    public RectTransform target;
    public Button btn;
    interface IB
    {
        public void Updatee();
    }
    class A : IReference
    {
        public A()
        {
            Debug.Log("草泥马");
        }

        public void Clear()
        {
            
        }
    }


    private void Update()
    {
        A a1 = ReferencePool.Acquire<A>();
        ReferencePool.ReleaseReference(a1);

        //mask.SetTarget(target, 200f, 200f,GuideMaskImage.MaskType.Circle);

        //btn.onClick.AddListener(() =>
        //{
        //    Debug.Log("穿透了");
        //});

        //mask._targetBoundsMax = 
    }
}