using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Event;
using GameFrameWork.UI;
using UnityEngine;

public class TaskTriggerStory_1002 : BaseTaskTrigger
{
    public TaskTriggerStory_1002(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        m_Pit = SceneEntityMgr.instance.GetSceneBuildingByName("Pit");
        m_Danmd = SceneEntityFactory.CreateRole("Damnd", "Character/Damnd.prefab", 1f, new Vector2(4.7f, -0.08f));
        m_Danmd.SetDir(-1);
        PlayerMgr.instance.canContrl = false;
        PlayerMgr.instance.player.AutoMoveToPos(new Vector2(3.2f, -0.27f), OnAutoMove1);
        m_Pit.gameObject.SetActiveSelf(false);
        EventMgr.instance.Subscribe(EventDefine.StageEnterStartEvent, OnStageEnterStart);
    }

    private void OnAutoMove1()
    {
        PlayerMgr.instance.player.onDropEvent.AddListener(() =>
        {
            PlayerMgr.instance.player.UpdatePosZ(0.1f);
        });

        PlayerMgr.instance.player.onGroundEvent.AddListener(() =>
        {
            PlayerMgr.instance.player.AutoMoveToPos(new Vector2(4.36f, -0.27f), OnAutoMove2);
        });

        PlayerMgr.instance.Jump(Vector2.right, false, true);
    }

    private void OnAutoMove2()
    {
        m_Pit.gameObject.SetActiveSelf(true);
        m_Danmd.AutoMoveToPos(new Vector2(5.4f, -0.08f), () =>
        {
            m_Danmd.Release();
            m_Danmd = null;
        });

        PlayerMgr.instance.player.PlayAnimation(AnimName.JumpDown);
        AudioMgr.instance.FadeBgm(0, 0.3f, 0.7f);
        PlayerMgr.instance.player.transform.DOLocalMoveY(-0.85f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Complete();
        });
    }

    private void OnStageEnterStart(object sender, GameEventArgs e)
    {
        EventMgr.instance.UnSubscribe(EventDefine.StageEnterStartEvent, OnStageEnterStart);
        AudioMgr.instance.PauseBgm();
        AudioMgr.instance.FadeBgm(1, 0, 0.1f);
        UIMgr.instance.Get(UINames.MainPanel).Hide();
        PlayerMgr.instance.player.gameObject.SetActiveSelf(false);
    }

    public override void Trigger()
    {

    }

    private BaseSceneObject m_Pit = null;
    private BaseRole m_Danmd = null;
}
