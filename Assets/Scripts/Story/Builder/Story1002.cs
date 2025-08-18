using DG.Tweening;
using UnityEngine;

public class Story1002 : BaseStoryBuilder
{
    public override void BuildStory()
    {
        SceneObjectActive(1, "Pit", false);
        RolePos(1, 2005, new Vector2(4.7f, -0.08f));
        RoleIdle(1, 2005, -1);
        RoleMove(1, -1, new Vector2(3.2f, -0.27f));
        RoleJump(1, -1, Vector2.right, 0.1f);
        RoleMove(1, -1, new Vector2(4.36f, -0.27f));
        RoleMove(1, 2005, new Vector2(5.4f, -0.08f));
        SceneObjectActive(1, "Pit", true);
        RoleAnim(1, -1, AnimName.JumpDown, -1, 1);
        RolePositionAnim(1, -1, 2, Vector3.up * -0.85f, 1, Ease.Linear);
        FadeBgm(1, 0, 0.3f, 0.7f);
        PauseBgm(1);
    }
}