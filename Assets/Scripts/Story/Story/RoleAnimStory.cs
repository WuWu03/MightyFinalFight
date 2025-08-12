using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;

public class RoleAnimStory : BaseStory
{
    public static RoleAnimStory Create(bool isWaitComplete, int roleId, string animName, int playTime, float playSpeed)
    {
        RoleAnimStory roleAnimStory = ReferencePool.Acquire<RoleAnimStory>();
        roleAnimStory.isWaitComplete = isWaitComplete;
        roleAnimStory.m_RoleId = roleId;
        roleAnimStory.m_AnimName = animName;
        roleAnimStory.m_PlayTime = playTime;
        roleAnimStory.m_PlaySpeed = playSpeed;
        return roleAnimStory;
    }

    public override bool IsStoryComplete()
    {
        if (m_PlayTime > 0)
        {
            return isPlaying && m_Role != null && m_Role.IsPlayComplete();
        }

        return isPlaying;
    }

    protected override void OnPlayStory()
    {
        if (m_RoleId < 0)
        {
            m_Role = PlayerMgr.instance.player;
        }
        else
        {
            string roleName = StringUtil.Append("StoryRole_", m_RoleId.ToString());
            m_Role ??= EntityMgr.instance.FindEntity<BaseRole>(roleName);
            m_Role ??= EntityMgr.instance.GetEntity<BaseRole>(roleName);
            RoleConfigData roleConfigData = ConfigDataSheet.roleConfigDatas.GetConfigDataById(m_RoleId);
            m_Role.SetAsset(roleConfigData.assetName);
        }

        m_Role.PlayAnimation(m_AnimName, m_PlayTime, m_PlaySpeed);
    }

    protected override void OnPauseStory()
    {
        m_Role?.Pause();
    }

    protected override void OnResumeStory()
    {
        m_Role?.Resume();
    }

    protected override void OnClear()
    {
        if (m_RoleId > 0)
        {
            m_Role?.Release();
        }

        m_RoleId = 0;
        m_AnimName = string.Empty;
        m_PlayTime = 0;
        m_PlaySpeed = 0;
        m_Role = null;
    }

    private int m_RoleId = 0;
    private string m_AnimName = string.Empty;
    private int m_PlayTime = 0;
    private float m_PlaySpeed = 0;
    private BaseRole m_Role = null;
}
