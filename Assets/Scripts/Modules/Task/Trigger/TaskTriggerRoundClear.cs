using GameFrameWork.Sound;
using GameFrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TaskTriggerRoundClear : BaseTaskTrigger
{
    public TaskTriggerRoundClear(TaskConfigData data) : base(data)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SoundMgr.instance.PlayBGM(ResDefine.AudioClipPath, "BGM/bgm15Clear", false);

        PlayerMgr.instance.player.currCtrl.Move(Vector2.zero);
        UIMgr.instance.Open<RoundClearPanel>();
        m_PlayTimer = Time.time;
    }

    public override void Trigger()
    {
        base.Trigger();

        if(m_PlayTimer > 0 && Time.time - m_PlayTimer >= 3.76)
        {
            UIMgr.instance.Open<LoadPanel>().DOFade(0f, 1f, 0.3f, 0.5f, () =>
            {
                UIMgr.instance.Close<LoadPanel>();
                UIMgr.instance.Close<RoundClearPanel>();
                UIMgr.instance.Open<StagePanel>();
            });

            Complete();
        }
    }

    private float m_PlayTimer = 0f;
}
