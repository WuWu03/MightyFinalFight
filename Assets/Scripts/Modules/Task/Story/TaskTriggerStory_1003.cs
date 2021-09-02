using DG.Tweening;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utility;
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
        PlayerMgr.Ins.CanContrl = false;
        SoundMgr.Ins.StopBGM();
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/FallDownHigh");
        UIMgr.Ins.GetPanel<MainPanel>().Hide();

        int sourceId = m_TaskData.Targets[0].SourceID;
        int entityId = m_TaskData.Targets[0].EntityID;
        int hp = m_TaskData.Targets[0].Hp;
        int attack = m_TaskData.Targets[0].AttackValue;
        int defense = m_TaskData.Targets[0].DefenseValue;
        int hpBarWidth = m_TaskData.Targets[0].HpBarWidth;
        Vector2Int pos = m_TaskData.Targets[0].Pos;
        m_Boss = SceneEntityMgr.Ins.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
        m_Boss.CurrCtrl.Stop();
    }

    public override void Trigger()
    {
        if(m_Boss.IsResComplete && !m_BossState)
        {
            m_BossState = true;
            m_Boss.CurrCtrl.Stop();
            m_Boss.FsmMachine.ChangeDefaultState();
            m_Boss.SetDir(-1);
        }

        if (PlayerMgr.Ins.Player.IsResComplete && !m_IsSwoon)
        {
            m_IsSwoon = true;
            Rect vision = CameraMgr.Ins.GetVision();
            PlayerMgr.Ins.Player.SetPos2(vision.xMin + 0.5f, vision.yMax);
            PlayerMgr.Ins.Player.PlayAnimation(AnimName.SwoonUp);
            PlayerMgr.Ins.Player.transform.DOMoveY(-0.6f, 2.2f).SetEase(Ease.Linear).OnComplete(OnPlayComplete);
        }
    }

    private void OnPlayComplete()
    {
        PlayerMgr.Ins.Player.PlayAnimation(AnimName.SwoonDown);
        Timer.Register(1, MoveTo);
    }

    private void MoveTo()
    {
        SoundMgr.Ins.StartBGM();
        PlayerMgr.Ins.Player.SetPos(PlayerMgr.Ins.Player.transform.localPosition);
        PlayerMgr.Ins.Player.FsmMachine.ChangeState<RoleAwaken>();
        GameObject black = GameObject.Find("Black");
        MainPanel mainPanel = UIMgr.Ins.GetPanel<MainPanel>();
        CanvasGroup group = mainPanel.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        mainPanel.Show();
        group.DOFade(1, 1);

        black.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
        {
            black.SetActive(false);
            PlayerMgr.Ins.Player.AutoMoveToPos(new Vector2(0.8f, -0.6f));
        });
    }

    private BaseEnemy m_Boss = null;
    private bool m_IsSwoon = false;
    private bool m_BossState = false;
}
