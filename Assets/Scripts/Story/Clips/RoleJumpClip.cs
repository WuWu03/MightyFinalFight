using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;
using UnityEngine;

public class RoleJumpClip : BaseClip
{
    public static RoleJumpClip Create(int roleId, Vector2 dir, float posZ)
    {
        RoleJumpClip roleJumpStory = ReferencePool.Acquire<RoleJumpClip>();
        roleJumpStory.m_RoleId = roleId;
        roleJumpStory.m_Dir = dir;
        roleJumpStory.m_PosZ = posZ;
        return roleJumpStory;
    }

    public override bool IsComplete()
    {
        return isPlaying && m_Role.isInGround;
    }

    protected override void OnClear()
    {
        if (m_RoleId > 0 && m_Role.GetType() != typeof(BaseEnemy))
        {
            m_Role.Release();
        }

        m_RoleId = 0;
        m_Dir = Vector2.zero;
        m_PosZ = 0f;
        m_Role = null;
    }

    protected override void OnPause()
    {
        m_Role.Pause();
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

        m_Role.onDropEvent.AddListener(OnDropEvent);
        m_Role.Jump(m_Dir, false, true);
    }

    protected override void OnResume()
    {
        m_Role.Resume();
    }

    private void OnDropEvent()
    {
        m_Role.onDropEvent.RemoveListener(OnDropEvent);
        m_Role.UpdatePosZ(m_PosZ);
    }

    private BaseRole m_Role;
    private int m_RoleId = 0;
    private Vector2 m_Dir = Vector2.zero;
    private float m_PosZ = 0f;
}
