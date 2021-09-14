using GameFrameWork.GameEntity;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.Utility;
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
        m_Pit = SceneEntityMgr.Ins.GetSceneBuildingByName("Pit");
        m_Danmd = SceneEntityFactory.CreateRole("Damnd", "Character/Damnd", 1f, new Vector2(4.7f, -0.08f));
        m_Danmd.SetDir(-1);
        PlayerMgr.Ins.CanContrl = false;
        PlayerMgr.Ins.Player.AutoMoveToPos(new Vector2(3.2f, -0.27f), OnAutoMove1);
        m_Pit.SetActive(false);
    }

    private void OnAutoMove1()
    {
        PlayerMgr.Ins.Player.OnDropEvent.AddListener(() =>
        {
            PlayerMgr.Ins.Player.UpdatePosY(-0.2f);
        });

        PlayerMgr.Ins.Player.OnGroundEvent.AddListener(() =>
        {
            PlayerMgr.Ins.Player.AutoMoveToPos(new Vector2(4.36f, -0.2f), OnAutoMove2);
        });

        PlayerMgr.Ins.Jump(Vector2.right, false, true);
    }

    private void OnAutoMove2()
    {
        m_Pit.SetActive(true);
        m_Danmd.AutoMoveToPos(new Vector2(5.4f, -0.08f), () =>
        {
            m_Danmd.Release();
            m_Danmd = null;
        });

        PlayerMgr.Ins.Player.PlayAnimation(AnimName.JumpDown);
        SoundMgr.Ins.FadeBGM(0, 0.3f, 0.7f);
        PlayerMgr.Ins.Player.transform.DOLocalMoveY(-0.85f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SceneMgr.Ins.LoadSceneSuccessEvent += OnSceneLoaded;
            PlayerMgr.Ins.Player.SetActive(false);
            Complete();
        });
    }

    private void OnSceneLoaded(LoadSceneSuccessEventArgs t)
    {
        SoundMgr.Ins.PauseBGM();
        SoundMgr.Ins.FadeBGM(1, 0, 0.1f);
        UIMgr.Ins.GetPanel<MainPanel>().Hide();
    }

    public override void Trigger()
    {

    }

    private BaseSceneObject m_Pit = null;
    private BaseRole m_Danmd = null;
}
