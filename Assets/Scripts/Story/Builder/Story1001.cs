using UnityEngine;

public class Story1001 : BaseStoryBuilder
{
    public override void BuildStory()
    {
        RoleIdle(1, -1, 1);
        WaitTime(1, 1);
        SceneObjectActive(1, "WoodDoorClose", false);
        PlaySe(1, SoundName.Break);
        WaitTime(1, 1);
        RoleMove(1, -1, new Vector2(-3.5f, -0.28f));
    }
}