using GameFrameWork.GameEntity;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.Utilities;
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
 
        Timer.Register(1, () =>
        {
            SceneEntityMgr.instance.GetSceneBuildingByName("WoodDoorClose").SetActive(false);
            SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/Break");
        });

        Timer.Register(2, () => 
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
