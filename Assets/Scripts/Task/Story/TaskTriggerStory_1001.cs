using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Timer;
using GameFrameWork.Utils;
using UnityEngine;

public class TaskTriggerStory_1001 : BaseTaskTrigger
{
    public TaskTriggerStory_1001(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        base.Enter();
        m_IsAutoMove = false;

        PlayerMgr.instance.player.SetDir(1);
        PlayerMgr.instance.playerCtrl.Move(Vector2.zero);

        TimerMgr.instance.Register(1, () =>
        {
            SceneEntityMgr.instance.GetSceneBuildingByName("WoodDoorClose").gameObject.SetActiveSelf(false);
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Break));
        });

        TimerMgr.instance.Register(2, () => 
        {
            PlayerMgr.instance.player.AutoMoveToPos(new Vector2(-3.5f, -0.28f));
            m_IsAutoMove = true;
        });
    }

    public override void Trigger()
    {
        base.Trigger();

        if(m_IsAutoMove && !PlayerMgr.instance.player.isAutoMove)
        {
            m_IsAutoMove = false;
            Complete();
        }
    }

    private bool m_IsAutoMove = false;
}
