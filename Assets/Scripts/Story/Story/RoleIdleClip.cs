using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleIdleClip : BaseClip
{
    public static RoleIdleClip Create(int roleId, int dir)
    {
        RoleIdleClip roleIdleStory = ReferencePool.Acquire<RoleIdleClip>();
        roleIdleStory.m_RoleId = roleId;
        roleIdleStory.m_Dir = dir;
        return roleIdleStory;
    }

    public override bool IsComplete()
    {
        return isPlaying;
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

        m_Role.SetDir(m_Dir);
        m_Role.Move(Vector2.zero);
    }

    protected override void OnPause()
    {

    }

    protected override void OnResume()
    {

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
    private int m_Dir = -1;
}
