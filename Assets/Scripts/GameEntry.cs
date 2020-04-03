using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork;
using FrameWork.Camera;

namespace Runtime
{
    public class GameEntry : FrameWorkEntry
    {
        protected override void OnInit()
        {
            ObjectMsgCenter.Init();
            DataHelper.Init();
            StaticConfig.InitConfig();
        }

        protected override void OnStartGame()
        {
            PlayerMgr.Ins.InitPlayer(0);
            CameraMgr.Ins.SetTarget(PlayerMgr.Ins.Player.transform);
            StageMgr.Ins.Enter(1001);
        }
    }
}