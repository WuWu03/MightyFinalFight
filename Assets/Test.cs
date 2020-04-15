using FrameWork.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FrameWork.Event;
using FrameWork.Sound;

public class Test : MonoBehaviour
{
    public Button btn1;
    public Button btn2;
    public Button btn3;
    private void Awake()
    {
        UIEventListener.Get(btn1.gameObject).onClick.AddListener(onClick1);
        UIEventListener.Get(btn2.gameObject).onClick.AddListener(onClick2);
        UIEventListener.Get(btn3.gameObject).onClick.AddListener(onClick3);
        //EventTriggerListener.Get(btn.gameObject).onPress.AddListener(onPress);

        //EventTriggerListener.Get(btn.gameObject).onDoubleClick.AddListener(onDoubleClick);
        //EventManager.Init();

        //AnimationEvent @event = new AnimationEvent();
        //@event.functionName = "Attack";
        //@event.objectReferenceParameter = btn;
        //@event.time = 0.08f;

        //AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        //for (int i = 0; i < clips.Length; i++)
        //{
        //    if (clips[i].name.Equals("attack"))
        //    {
        //        clips[i].AddEvent(@event);
        //        break;
        //    }
        //}

        //anim.Play("attack_1", 0);
    }

    private void Start()
    {

        //EventManager.Ins.Subscribe(1,OnSub);
    }
    
    private void onClick1(GameObject go, PointerEventData eventData)
    {
        UIMgr.Ins.Open<RoleSelectPanel>();
    }


    private void onClick2(GameObject go, PointerEventData eventData)
    {
        UIMgr.Ins.Close<RoleSelectPanel>();
    }

    private void onClick3(GameObject go, PointerEventData eventData)
    {
        UIMgr.Ins.Open<MainPanel>();
    }
    private void onPress(GameObject go, PointerEventData eventData)
    {
        Debug.Log("OnPress");
    }

    private void onDoubleClick(GameObject go, PointerEventData eventData)
    {
        Debug.Log("OnDoubleClick");
    }

    public void Attack(object data)
    {
        Debug.Log("草年末" + "," + (data is Button));
    }

    private void OnSub(object seneder,GameEventArgs args)
    {
        Debug.Log(args.ID);
    }
}
