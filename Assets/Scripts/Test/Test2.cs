using GameFrameWork.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    public GuidMask mask;
    public RectTransform target;
    public Button btn;
    private void Start()
    {
        mask.SetTarget(target, 200f, 200f);

        btn.onClick.AddListener(() =>
        {
            Debug.Log("穿透了");
        });

        //mask._targetBoundsMax = 
    }
}