using FrameWork.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FrameWork.Event;
using FrameWork.Sound;

public class Test : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource source;
    public Button btn;
    public Animator anim;
    private void Awake()
    {
        EventTriggerListener.Get(btn.gameObject).onClick.AddListener(onClick);
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
    
    private void onClick(GameObject go, PointerEventData eventData)
    {
        //EventManager.Ins.Dispatch(this,new GameEventArgs(){ID = 1});
        //Debug.Log("OnClcik");
        //SoundMgr.Ins.PlaySound("CodyBullet");
        //source.PlayOneShot(UnityEngine.Object.Instantiate(clip));
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
