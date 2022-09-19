using DG.Tweening;
using GameFrameWork.Camera;
using GameFrameWork.GameEntity;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
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
        SoundMgr.instance.PauseBGM();
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/FallDownHigh");
        UIMgr.instance.GetPanel<MainPanel>().Hide();

        int sourceId = m_TaskData.Targets[0].SourceID;
        int entityId = m_TaskData.Targets[0].EntityID;
        int hp = m_TaskData.Targets[0].Hp;
        int attack = m_TaskData.Targets[0].AttackValue;
        int defense = m_TaskData.Targets[0].DefenseValue;
        int hpBarWidth = m_TaskData.Targets[0].HpBarWidth;
        Vector2Int pos = m_TaskData.Targets[0].Pos;
        m_Boss = SceneEntityMgr.instance.CreateEnemy(sourceId, entityId, hp, attack, defense, hpBarWidth, pos);
        m_Boss.currCtrl.Stop();
    }

    public override void Trigger()
    {
        if (m_Boss.isResComplete && !m_BossState)
        {
            m_BossState = true;
            m_Boss.currCtrl.Stop();
            m_Boss.ChangeDefaultState();
            m_Boss.SetDir(-1);
        }

        if (PlayerMgr.instance.player.isResComplete && !m_IsSwoon)
        {
            m_IsSwoon = true;
            Rect vision = CameraMgr.instance.GetVision();
            PlayerMgr.instance.player.SetActive(true);
            PlayerMgr.instance.player.SetPosXY(vision.xMin + 0.5f, vision.yMax);
            PlayerMgr.instance.player.PlayAnimation(AnimName.SwoonUp);
            PlayerMgr.instance.player.transform.DOMoveY(-0.6f, 2.2f).SetEase(Ease.Linear).OnComplete(OnPlayComplete);
        }
    }

    private void OnPlayComplete()
    {
        PlayerMgr.instance.player.PlayAnimation(AnimName.SwoonDown);
        Timer.Register(1, MoveTo);
    }

    private void MoveTo()
    {
        SoundMgr.instance.StartBGM();
        PlayerMgr.instance.player.SetPos2(PlayerMgr.instance.player.transform.localPosition);
        PlayerMgr.instance.player.ChangeState<RoleAwaken>();
        GameObject black = GameObject.Find("Black");
        MainPanel mainPanel = UIMgr.instance.GetPanel<MainPanel>();
        CanvasGroup group = mainPanel.GetComponent<CanvasGroup>();
        group.alpha = 0f;
        mainPanel.Show();
        group.DOFade(1, 1);

        black.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
        {
            black.SetActive(false);
            PlayerMgr.instance.player.AutoMoveToPos(new Vector2(0.8f, -0.6f),()=> 
            {
                PlayerMgr.instance.canContrl = true;
                m_Boss.currCtrl.Start();
            });
        });
    }

    private BaseEnemy m_Boss = null;
    private bool m_IsSwoon = false;
    private bool m_BossState = false;
}
