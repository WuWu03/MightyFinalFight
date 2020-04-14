using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using FrameWork.Camera;
using FrameWork.UI;


public class GameEntry : FrameWorkEntry
{
    protected override void OnInit()
    {
        ObjectMsgCenter.Init();
        StaticConfig.InitConfig();

        UIMgr.Ins.AddPanelMap<RoleSelectPanelCtrl>("RoleSelectPanel");
        UIMgr.Ins.AddPanelMap<MainPanelCtrl>("MainPanel");
    }

    protected override void OnStartGame()
    {
        UIMgr.Ins.Open<RoleSelectPanel>();
    }
}