using GameFrameWork.Audio;
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
        AudioMgr.instance.PlayBGM(AssetPathDefine.AudioClipPath, SoundName.Bgm15Clear, false, 1, 0.3f, true);

        PlayerMgr.instance.player.currCtrl.Move(Vector2.zero);
        UIMgr.instance.Open<RoundClearPanel>();
        m_PlayTimer = Time.time;
    }

    public override void Trigger()
    {
        base.Trigger();

        if (m_PlayTimer > 0 && Time.time - m_PlayTimer >= 3.76)
        {
            LoadPanel loadPanel = UIMgr.instance.Open<LoadPanel>() as LoadPanel;

            loadPanel.DOFade(0f, 1f, 0.3f, 0.5f, () =>
            {
                UIMgr.instance.Close<RoundClearPanel>();
                UIMgr.instance.Open<StagePanel>();
            });

            loadPanel.DOFade(1f, 0f, 0.3f, 0.5f, () =>
            {
                UIMgr.instance.Close<LoadPanel>();
            });

            Complete();
        }
    }

    private float m_PlayTimer = 0f;
}
