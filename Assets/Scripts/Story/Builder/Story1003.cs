using DG.Tweening;
using GameFrameWork;
using UnityEngine;

public class Story1003 : BaseStoryBuilder
{
    public override void BuildStory()
    {
        PauseBgm(1);
        UIShowHide(1, typeof(MainView), false);
        RoleIdle(1, 200501, -1);
        PauseEnemy(1, 200501);;
        PlaySe(1, SoundName.FallDownHigh, 1);
        RolePos(1, -1, new Vector2(-2f, 1.2f));
        RoleAnim(1, -1, AnimName.SwoonUp, -1, 1);
        RolePositionAnim(1, -1, 2, Vector3.up * -0.6f, 2.2f, Ease.Linear);
        RoleAnim(1,-1, AnimName.SwoonDown,-1,1);
        WaitTime(1, 1);
        PlayBgm(1, "BGM/bgm06Boss_Start.ogg", false, 1, 0, true);
        PlayBgm(1, "BGM/bgm06Boss_Loop.ogg", true, 1, 0, false);
        ResumeBgm(1);
        Callback(1, ShowBlack);
        WaitTime(1, 1);
        RoleAnim(1, -1, AnimName.Awaken, 1, 0.2f);
        RoleAnim(1, -1, AnimName.Idle, 1, 1f);
        RolePos(1, -1, new Vector2(-2f, -0.6f));
        RoleIdle(1, -1, 1);
        WaitTime(-1, 1);
        RoleMove(1, -1, new Vector2(0.8f, -0.6f));
        Talk(1, 1001);
        UIShowHide(1, typeof(MainView), true);
        ResumeEnemy(1, 200501);
    }

    private void ShowBlack()
    {
        GameObject black = GameObject.Find("Black");

        black.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(() =>
        {
            black.SetActiveSelf(false);
        });
    }
}
