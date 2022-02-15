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
using UnityEngine.U2D;
using DragonBones;
using System;

public class Test : MonoBehaviour
{
    public ScrollListEX ex;
    private void Awake()
    {
        ex.Init(50, 0, (ListItem item,int index, bool act) => 
        {
            //item.ItemObject.SetActive(true);
        }, null);
  

    }


    private void Start()
    {

    }

    private void Update()
    {
 
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


}