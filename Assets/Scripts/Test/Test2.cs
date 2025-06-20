using GameFrameWork.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{
    public GuideMaskImage mask;
    public RectTransform target;
    public Button btn;
    private void Start()
    {
        mask.SetTarget(target, 200f, 200f,GuideMaskImage.MaskType.Circle);

        btn.onClick.AddListener(() =>
        {
            Debug.Log("穿透了");
        });

        //mask._targetBoundsMax = 
    }
}