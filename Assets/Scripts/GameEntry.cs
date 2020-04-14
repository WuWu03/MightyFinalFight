using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using FrameWork.Camera;
using FrameWork.UI;

namespace Runtime
{
    public class GameEntry : FrameWorkEntry
    {
        protected override void OnInit()
        {
            ObjectMsgCenter.Init();
            StaticConfig.InitConfig();

            UIMgr.Ins.AddPanelMap<RoleSelectPanelCtrl>("RoleSelectPanel");
        }

        protected override void OnStartGame()
        {
            //PlayerMgr.Ins.InitPlayer (1001);
            //CameraMgr.Ins.SetTarget(PlayerMgr.Ins.Player.transform);
            //StageMgr.Ins.Enter(1001);
            UIMgr.Ins.Open<RoleSelectPanel>();
        }
    }
}