using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleAnimClip : BaseClip
{
    private int m_RoleId;
    private string m_AnimName = string.Empty;
    private int m_PlayTime;
    private float m_PlaySpeed;
    private BaseRole m_Role;
    
    public static RoleAnimClip Create(int roleId, string animName, int playTime, float playSpeed)
    {
        RoleAnimClip roleAnimStory = ReferencePool.Acquire<RoleAnimClip>();
        roleAnimStory.m_RoleId = roleId;
        roleAnimStory.m_AnimName = animName;
        roleAnimStory.m_PlayTime = playTime;
        roleAnimStory.m_PlaySpeed = playSpeed;
        return roleAnimStory;
    }

    public override bool IsComplete()
    {
        if (m_PlayTime > 0)
        {
            return isPlaying && m_Role is not null && m_Role.IsAllAnimationComplete();
        }

        return base.IsComplete();
    }

    protected override void OnPlay()
    {
        if (m_RoleId < 0)
        {
            m_Role = PlayerMgr.instance.player;
        }
        else
        {
            m_Role = SceneEntityMgr.instance.GetEnemyById(m_RoleId);

            if (m_Role is null)
            {
                string roleName = StringUtil.Append("StoryRole_", m_RoleId.ToString());
                m_Role = GameEntry.entityMgr.FindEntity<BaseRole>(roleName);

                if (m_Role is null)
                {
                    RoleConfigData roleConfigData = GameEntry.configDataMgr.Get<RoleConfigData>().GetConfigDataById(m_RoleId);
                    m_Role = SceneEntityFactory.CreateRole(roleName, roleConfigData.assetName, 1f, Vector2.zero);
                }
            }
        }

        m_Role.PlayAnimation(m_AnimName, m_PlayTime, m_PlaySpeed);
        Complete();
    }

    protected override void OnPause()
    {
        m_Role.Pause();
    }

    protected override void OnResume()
    {
        m_Role.Resume();
    }

    protected override void OnClear()
    {
        if (m_RoleId > 0 && m_Role.GetType() != typeof(BaseEnemy))
        {
            m_Role.Release();
        }

        m_RoleId = 0;
        m_AnimName = string.Empty;
        m_PlayTime = 0;
        m_PlaySpeed = 0;
        m_Role = null;
    }
}
