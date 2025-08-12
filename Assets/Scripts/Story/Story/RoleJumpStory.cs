using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.GameEntity;
using GameFrameWork.Utils;
using System.Drawing.Printing;
using UnityEngine;

public class RoleJumpStory : BaseStory
{
    public static RoleJumpStory Create(int roleId,Vector2 dir, float posZ)
    {
        RoleJumpStory roleJumpStory = ReferencePool.Acquire<RoleJumpStory>();
        roleJumpStory.m_RoleId = roleId;
        roleJumpStory.m_Dir = dir;
        roleJumpStory.m_PosZ = posZ;
        return roleJumpStory;
    }

    public override bool IsStoryComplete()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnClear()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnPauseStory()
    {
        throw new System.NotImplementedException();
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

        m_Role.onDropEvent.AddListener(OnDropEvent);
        m_Role.currCtrl.Jump(Vector2.right, false, true);
    }

    private void OnDropEvent()
    {
        PlayerMgr.instance.player.UpdatePosZ(m_PosZ);
    }

    protected override void OnResumeStory()
    {
        throw new System.NotImplementedException();
    }

    private BaseRole m_Role;
    private int m_RoleId = 0;
    private Vector2 m_Dir = Vector2.zero;
    private float m_PosZ = 0f;
}
