using GameFrameWork.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameFrameWork.Event;
using GameFrameWork.Sound;
using UnityEngine.AI;
using GameFrameWork.Resources;
using GameFrameWork.Scene;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Test : MonoBehaviour
{
    public Button btn1;
    public Button btn2;
    //public Button btn3;
    //public GameObject parent;
    //public GameObject item;
    //public ScrollRect scroll;
    //public NavMeshAgent agent;
    public class TestEventArg : GameEventArgs
    {
        public string fuck;

        public override void Clear()
        {
            fuck = string.Empty;
        }
    }


    private void Awake()
    {
        ResMgr.Init(this.gameObject);
        SceneMgr.Init(this.gameObject);
        DontDestroyOnLoad(gameObject);
        //EventMgr.Init(parent);
        UIEventListener.Get(btn1.gameObject).onClick.AddListener(onClick1);
        UIEventListener.Get(btn2.gameObject).onClick.AddListener(onClick2);
        //UIEventListener.Get(btn3.gameObject).onClick.AddListener(onClick3);
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
       // EventMgr.Ins.Subscribe(1,OnSub);
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hitinfo;
        //    bool racast = Physics.Raycast(ray, out hitinfo, 100f, LayerMask.GetMask("Map"));

        //    if (racast)
        //    {
        //        agent.destination = hitinfo.point;
        //    }
        //}

       
    }

    private void onClick1(GameObject go, PointerEventData eventData)
    {
        //wrap.SetItemCount(10);
        //UIMgr.Ins.Open<RoleSelectPanel>();
        //TestEventArg e = new TestEventArg();
        //e.Id = 1;
        //e.fuck = "fuck";
        //EventMgr.Ins.Dispatch(this, e);

        // UnityEditor.SceneAsset ss = ResMgr.Ins.LoadAsset<UnityEditor.SceneAsset>("ArtResources/Scene/Stage1_1", true);
        // EditorBuildSettings.scenes = 
        // SceneManager.SetActiveScene()
        // SceneManager.LoadScene("Stage1_1");

        SceneMgr.Ins.LoadSceneAsync("Stage1_1");
    }

    private void onClick2(GameObject go, PointerEventData eventData)
    {
        SceneMgr.Ins.LoadSceneAsync("Main");
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

    private void OnSub(object seneder, GameEventArgs args)
    {
        TestEventArg e = args as TestEventArg;
        Debug.Log(args.Id);
        Debug.Log(e.fuck);
    }
}