using WuWuFramework;
using WuWuFramework.ConfigData;
using WuWuFramework.Utils;
using UnityEngine;

public class RolePosClip : BaseClip
{
    private BaseRole m_Role;
    private int m_RoleId = -1;
    private Vector2 m_Pos = Vector2.zero;
    
    public static RolePosClip Create(int roleId, Vector2 pos)
    {
        RolePosClip clip = ReferencePool.Acquire<RolePosClip>();
        clip.m_RoleId = roleId;
        clip.m_Pos = pos;
        return clip;
    }

    protected override void OnClear()
    {
        if (m_RoleId > 0 && m_Role.GetType() != typeof(BaseEnemy))
        {
            m_Role.Release();
        }

        m_RoleId = 0;
        m_Pos = Vector2.zero;
        m_Role = null;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        if(m_RoleId == -1)
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
                    RoleConfigData roleConfigData = GameEntry.configDataMgr.Get<RoleConfigData>().Get(m_RoleId);
                    m_Role = SceneEntityFactory.CreateRole(roleName, roleConfigData.assetName, 1f, Vector2.zero);
                }
            }
        }

        m_Role.SetPos2(m_Pos);
        Complete();
    }

    protected override void OnResume()
    {

    }
}
