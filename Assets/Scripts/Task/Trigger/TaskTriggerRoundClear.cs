using GameFrameWork.Audio;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class TaskTriggerRoundClear : BaseTaskTrigger
{
    public TaskTriggerRoundClear(TaskConfigData data) : base(data)
    {
    }

    public override void Enter()
    {
        base.Enter();
        AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmClear), false, 1, 0.3f, true);
        PlayerMgr.instance.player.Move(Vector2.zero);
        UIMgr.instance.Open<RoundClearView>();
        m_PlayTimer = Time.time;
    }

    public override void Trigger()
    {
        base.Trigger();

        if (m_PlayTimer > 0 && Time.time - m_PlayTimer >= 3.76)
        {
            m_PlayTimer = -1;
            LoadMgr.instance.DOFadeBlack(OnLoadFadeComplete);
        }
    }

    private void OnLoadFadeComplete()
    {
        Complete();
        UIMgr.instance.Close<RoundClearView>();
        UIMgr.instance.Open<StageView>();
    }

    private float m_PlayTimer = 0f;
}
