using DragonBones;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UnityArmatureComponent armature;
    //public GameObject mail1RedPoint;
    //public GameObject mail2RedPoint;
    //public GameObject mail3RedPoint;
    //public GameObject mail4RedPoint;
    //public GameObject mail5RedPoint;
    //public GameObject mail6RedPoint;

    //public Text txtMail1;
    //public Text txtMail2;
    //public Text txtMail3;
    //public Text txtMail4;
    //public Text txtMail5;
    //public Text txtMail6;

    //public Button mail4Btn;
    //public Button mail5Btn;
    //public Button mail6Btn;
    //public Button btnSet1;
    //public Button btnSet2;
    //public Button btnSet3;

    //public int count1 = 5;
    //public int count2 = 6;
    //public int count3 = 7;

    //string mail1 = "mail1";
    //string mail2 = "mail2";
    //string mail3 = "mail3";
    //string mail4 = "mail4";
    //string mail5 = "mail5";
    //string mail6 = "mail6";

    private void Awake()
    {
        //RedPointMgr.Init(gameObject);

        ////在实际开发中，整个游戏的红点树要在游戏初始化时全部构建出来
        ////声明mail1根节点，它的主key是mail1，无subKey，无父节点，红点类型是随着子节点变化
        //RedPointMgr.instance.Add(mail1, null, null, RedPointType.Enternal);
        ////声明mail2节点，它的主key是mail1，subKey是mail2，父节点是mail1，红点类型是随着子节点变化
        //RedPointMgr.instance.Add(mail1, mail2, mail1, RedPointType.Enternal);
        ////声明mail3节点，它的主key是mail1，subKey是mail3，父节点是mail2，红点类型是随着子节点变化
        //RedPointMgr.instance.Add(mail1, mail3, mail2, RedPointType.Enternal);
        ////声明mai4节点，它的主key是mail1，subKey是mail4，父节点是mail3，红点类型是点击即消失
        //RedPointMgr.instance.Add(mail1, mail4, mail3, RedPointType.Once);
        ////声明mai5节点，它的主key是mail1，subKey是mail5，父节点是mail3，红点类型是点击即消失
        //RedPointMgr.instance.Add(mail1, mail5, mail3, RedPointType.Once);
        ////声明mai5节点，它的主key是mail1，subKey是mail6，父节点是mail3，红点类型是点击即消失
        //RedPointMgr.instance.Add(mail1, mail6, mail3, RedPointType.Once);

        ////在实际开发中，初始化代码要写在对应UI界面的初始化函数中
        //RedPointMgr.instance.Init(mail1, mail1, OnMail1Show);
        //RedPointMgr.instance.Init(mail1, mail2, OnMail2Show);
        //RedPointMgr.instance.Init(mail1, mail3, OnMail3Show);
        //RedPointMgr.instance.Init(mail1, mail4, OnMail4Show, mail4Btn);
        //RedPointMgr.instance.Init(mail1, mail5, OnMail5Show, mail5Btn);
        //RedPointMgr.instance.Init(mail1, mail6, OnMail6Show, mail6Btn);

        //btnSet1.onClick.AddListener(OnBtnSet1Click);
        //btnSet2.onClick.AddListener(OnBtnSet2Click);
        //btnSet3.onClick.AddListener(OnBtnSet3Click);
    }

    private bool dead = false;
    private int frame = 0;
    private float timer = 0;
    private bool isDebug = false;
    private void FixedUpdate()
    {
        if (!dead)
        {
            timer = Time.time;
            dead = true;
            frame = 0;
            armature.animation.timeScale = 0.5f;
            armature.animation.Play("Dead", 1);
        }
        else
        {
            timer += Time.fixedDeltaTime;
            frame++;
            if (!armature.animation.GetState("Dead").isPlaying && !isDebug)
            {
                isDebug = true;
                UnityEngine.Debug.Log(frame + "," + timer + "," + armature.animation.animations["Dead"].duration / armature.animation.timeScale);
            }
        }
    }

    //private void OnMail1Show(RedPointState state, int data)
    //{
    //    mail1RedPoint.SetActiveSelf(state == RedPointState.Show);
    //    txtMail1.text = data.ToString();
    //}

    //private void OnMail2Show(RedPointState state, int data)
    //{
    //    mail2RedPoint.SetActiveSelf(state == RedPointState.Show);
    //    txtMail2.text = data.ToString();
    //}

    //private void OnMail3Show(RedPointState state, int data)
    //{
    //    mail3RedPoint.SetActiveSelf(state == RedPointState.Show);
    //    txtMail3.text = data.ToString();
    //}

    //private void OnMail4Show(RedPointState state, int data)
    //{
    //    mail4RedPoint.SetActiveSelf(state == RedPointState.Show);
    //    txtMail4.text = data.ToString();
    //}

    //private void OnMail5Show(RedPointState state, int data)
    //{
    //    mail5RedPoint.SetActiveSelf(state == RedPointState.Show);
    //    txtMail5.text = data.ToString();

    //}

    //private void OnMail6Show(RedPointState state, int data)
    //{
    //    mail6RedPoint.SetActiveSelf(state == RedPointState.Show);
    //    txtMail6.text = data.ToString();
    //}

    //private void OnBtnSet1Click()
    //{
    //    RedPointMgr.instance.SetState(mail1, mail4, count1 == 0 ? RedPointState.Hide : RedPointState.Show, count1);
    //}

    //private void OnBtnSet2Click()
    //{
    //    RedPointMgr.instance.SetState(mail1, mail5, count2 == 0 ? RedPointState.Hide : RedPointState.Show, count2);
    //}

    //private void OnBtnSet3Click()
    //{
    //    RedPointMgr.instance.SetState(mail1, mail6, count3 == 0 ? RedPointState.Hide : RedPointState.Show, count3);
    //}
}