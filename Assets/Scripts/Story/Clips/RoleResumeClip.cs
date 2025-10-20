using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleResumeClip : BaseClip
{
    private int m_RoleId;
    private BaseRole m_Role;
    
    public static RoleResumeClip Create(int entityId)
    {
        RoleResumeClip clip = ReferencePool.Acquire<RoleResumeClip>();
        clip.m_RoleId = entityId;
        return clip;
    }

    protected override void OnClear()
    {
        if (m_RoleId > 0 && m_Role.GetType() != typeof(BaseEnemy))
        {
            m_Role.Release();
        }

        m_RoleId = 0;
        m_Role = null;
    }

    protected override void OnPause()
    {

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
                    RoleConfigData roleConfigData = ConfigDataSheet.roleConfigDatas.GetConfigDataById(m_RoleId);
                    m_Role = SceneEntityFactory.CreateRole(roleName, roleConfigData.assetName, 1f, Vector2.zero);
                }
            }
        }

        m_Role.Resume();
        Complete();
    }

    protected override void OnResume()
    {

    }
}