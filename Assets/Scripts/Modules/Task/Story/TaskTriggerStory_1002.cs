using GameFrameWork.GameEntity;
using GameFrameWork.Audio;
using GameFrameWork.Timer;
using GameFrameWork.Utilities;
using System;
using UnityEngine;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Scene;

public class TaskTriggerStory_1002 : BaseTaskTrigger
{
    public TaskTriggerStory_1002(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        m_Pit = SceneEntityMgr.instance.GetSceneBuildingByName("Pit");
        m_Danmd = SceneEntityFactory.CreateRole("Damnd", "Character/Damnd", 1f, new Vector2(4.7f, -0.08f));
        m_Danmd.SetDir(-1);
        PlayerMgr.instance.canContrl = false;
        PlayerMgr.instance.player.AutoMoveToPos(new Vector2(3.2f, -0.27f), OnAutoMove1);
        m_Pit.SetActive(false);
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
        m_Pit.SetActive(true);
        m_Danmd.AutoMoveToPos(new Vector2(5.4f, -0.08f), () =>
        {
            m_Danmd.Release();
            m_Danmd = null;
        });

        PlayerMgr.instance.player.PlayAnimation(AnimName.JumpDown);
        AudioMgr.instance.FadeBGM(0, 0.3f, 0.7f);
        PlayerMgr.instance.player.transform.DOLocalMoveY(-0.85f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SceneMgr.instance.loadSceneSuccessEvent += OnSceneLoaded;
            PlayerMgr.instance.player.SetActive(false);
            Complete();
        });
    }

    private void OnSceneLoaded(LoadSceneSuccessEventArgs t)
    {
        AudioMgr.instance.PauseBGM();
        AudioMgr.instance.FadeBGM(1, 0, 0.1f);
        UIMgr.instance.Get<MainPanel>().Hide();
    }

    public override void Trigger()
    {

    }

    private BaseSceneObject m_Pit = null;
    private BaseRole m_Danmd = null;
}
