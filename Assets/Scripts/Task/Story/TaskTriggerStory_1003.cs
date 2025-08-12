using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Camera;
using GameFrameWork.Event;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class TaskTriggerStory_1003 : BaseTaskTrigger
{
    public TaskTriggerStory_1003(TaskConfigData data) : base(data)
    {

    }

    public override void Enter()
    {
        m_IsSwoon = false;
        m_BossState = false;
        PlayerMgr.instance.canContrl = false;
        PlayerMgr.instance.player.UpdatePosZ(0);
        AudioMgr.instance.PauseBgm();
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.FallDownHigh));
        UIMgr.instance.Get(UINames.MainPanel).Hide();

        int sourceId = m_TaskData.Targets[0].SourceID;
        int entityId = m_TaskData.Targets[0].EntityID;
        int hp = m_TaskData.Targets[0].Hp;
        int attack = m_TaskData.Targets[0].AttackValue;
        int defense = m_TaskData.Targets[0].DefenseValue;
        int hpBarWidth = m_TaskData.Targets[0].HpBarWidth;
        Vector2Int pos = m_TaskData.Targets[0].Pos;
        m_Boss = SceneEntityMgr.instance.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
        m_Boss.currCtrl.Stop();

        EventMgr.instance.Subscribe(EventDefine.TalkEndEvent, OnTalkEnd);
    }

    public override void Trigger()
    {
        if (m_Boss.isAssetLoadComplete && !m_BossState)
        {
            m_BossState = true;
            m_Boss.currCtrl.Stop();
            m_Boss.ChangeDefaultState();
            m_Boss.SetDir(-1);
        }

        if (PlayerMgr.instance.player.isAssetLoadComplete && !m_IsSwoon)
        {
            m_IsSwoon = true;
            Rect vision = CameraMgr.instance.GetVision();
            PlayerMgr.instance.player.gameObject.SetActiveSelf(true);
            PlayerMgr.instance.player.SetPosXY(vision.xMin + 0.5f, vision.yMax);
            PlayerMgr.instance.player.PlayAnimation(AnimName.SwoonUp);
            PlayerMgr.instance.player.transform.DOMoveY(-0.6f, 2.2f).SetEase(Ease.Linear).OnComplete(OnPlayComplete);
        }
    }

    private void OnPlayComplete()
    {
        PlayerMgr.instance.player.PlayAnimation(AnimName.SwoonDown);
        TimerMgr.instance.Register(1, MoveTo);
    }

    private void MoveTo()
    {
        AudioMgr.instance.ResumeBgm();
        PlayerMgr.instance.player.SetPos2(PlayerMgr.instance.player.transform.localPosition);
        PlayerMgr.instance.player.ChangeState<RoleAwaken>();
        GameObject black = GameObject.Find("Black");
        UIMgr.instance.Get(UINames.MainPanel).Show();

        black.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
        {
            black.SetActiveSelf(false);
            PlayerMgr.instance.player.AutoMoveToPos(new Vector2(0.8f, -0.6f), () =>
            {
                UIMgr.instance.Open(UINames.TalkPanel, m_TaskData.TalkID);
            });
        });
    }

    private void OnTalkEnd(object sender, GameEventArgs e)
    {
        m_Boss.currCtrl.Start();
        PlayerMgr.instance.canContrl = true;
        EventMgr.instance.UnSubscribe(EventDefine.TalkEndEvent, OnTalkEnd);
        Complete();
    }

    private BaseEnemy m_Boss = null;
    private bool m_IsSwoon = false;
    private bool m_BossState = false;
}
