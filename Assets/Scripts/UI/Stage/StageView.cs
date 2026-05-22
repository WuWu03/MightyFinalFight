/*
 * @Desc: Stage 模块 StageView 界面数据
 * @Date: 2025-10-11 12:19:46
 * @Author: GQY
 * @Note: 工具生成，请勿修改
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameFrameWork.UI;

public class StageView : UIBaseView<StageView, StageViewPresenter, StageViewSettings>
{
    //blue,GameObject
    public GameObject blueGo { get; private set; }
    //red,GameObject
    public GameObject redGo { get; private set; }
    //green,GameObject
    public GameObject greenGo { get; private set; }
    //imgMap,GameObject
    public GameObject imgMapGo { get; private set; }
    //heroPos,GameObject
    public GameObject heroPosGo { get; private set; }

    protected override void OnInitView(UIRefRoot root)
    {
        blueGo = root.objects[0] as GameObject;
        redGo = root.objects[1] as GameObject;
        greenGo = root.objects[2] as GameObject;
        imgMapGo = root.objects[3] as GameObject;
        heroPosGo = root.objects[4] as GameObject;
    }
}