using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleMoveStory : BaseStory
{
    public static RoleMoveStory Create(bool isWaitComplete, int roleId, Vector2 startPos, Vector2 endPos)
    {
        RoleMoveStory roleMoveStory = ReferencePool.Acquire<RoleMoveStory>();
        roleMoveStory.isWaitComplete = isWaitComplete;
        roleMoveStory.m_RoleId = roleId;
        roleMoveStory.m_StartPos = startPos;
        roleMoveStory.m_EndPos = endPos;
        return roleMoveStory;
    }

    public override bool IsStoryComplete()
    {
        return isPlaying && m_Role != null && !m_Role.isPause && !m_Role.isAutoMove;
    }

    protected override void OnPlayStory()
    {
        if (m_RoleId == -1)
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

        m_Role.SetPos2(m_StartPos);
        m_Role.AutoMoveToPos(m_EndPos);
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
        m_Role = null;
    }

    private int m_RoleId;
    private BaseRole m_Role = null;
    private Vector2 m_StartPos;
    private Vector2 m_EndPos;
}
