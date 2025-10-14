using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleMoveClip : BaseClip
{
    public static RoleMoveClip Create(int roleId, Vector2 endPos)
    {
        RoleMoveClip roleMoveStory = ReferencePool.Acquire<RoleMoveClip>();
        roleMoveStory.m_RoleId = roleId;
        roleMoveStory.m_EndPos = endPos;
        return roleMoveStory;
    }

    protected override void OnPlay()
    {
        if (m_RoleId == -1)
        {
            m_Role = PlayerMgr.instance.player;
        }
        else
        {
            m_Role = SceneEntityMgr.instance.GetEnemyById(m_RoleId);

            if (m_Role == null)
            {
                string roleName = StringUtil.Append("StoryRole_", m_RoleId.ToString());
                m_Role = EntityMgr.instance.FindEntity<BaseRole>(roleName);

                if (m_Role == null)
                {
                    RoleConfigData roleConfigData = ConfigDataSheet.roleConfigDatas.GetConfigDataById(m_RoleId);
                    m_Role = SceneEntityFactory.CreateRole(roleName, roleConfigData.assetName, 1f, Vector2.zero);
                }
            }
        }

        m_Role.AutoMove(m_EndPos, Complete);
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
        m_Role = null;
    }

    private int m_RoleId;
    private BaseRole m_Role = null;
    private Vector2 m_EndPos;
}
