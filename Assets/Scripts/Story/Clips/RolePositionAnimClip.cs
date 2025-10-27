using DG.Tweening;
using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Utils;
using UnityEngine;

public class RolePositionAnimClip : BaseClip
{
    private int m_RoleId = -1;
    private int m_AnimType;
    private Vector3 m_EndPos = Vector3.zero;
    private float m_Duration;
    private Ease m_Ease;
    private Tweener m_Tweener;
    private BaseRole m_Role;
    
    public static RolePositionAnimClip Create(int roleId, int animType, Vector3 endPos, float duration, Ease ease)
    {
        RolePositionAnimClip clip = ReferencePool.Acquire<RolePositionAnimClip>();
        clip.m_RoleId = roleId;
        clip.m_AnimType = animType;
        clip.m_EndPos = endPos;
        clip.m_Duration = duration;
        clip.m_Ease = ease;
        return clip;
    }

    protected override void OnClear()
    {
        if (m_RoleId > 0 && m_Role.GetType() != typeof(BaseEnemy))
        {
            m_Role.Release();
        }

        m_Tweener.Kill();
        m_RoleId = 0;
        m_Role = null;
        m_Tweener = null;
        m_EndPos = Vector3.zero;
        m_Duration = 0;
    }

    protected override void OnPause()
    {
        m_Tweener.Pause();
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

        if (m_AnimType == 1)
        {
            m_Tweener = m_Role.transform.DOLocalMoveX(m_EndPos.x, m_Duration).SetEase(m_Ease);
        }
        else if (m_AnimType == 2)
        {
            m_Tweener = m_Role.transform.DOLocalMoveY(m_EndPos.y, m_Duration).SetEase(m_Ease);
        }
        else if (m_AnimType == 3)
        {
            m_Tweener = m_Role.transform.DOLocalMoveZ(m_EndPos.z, m_Duration).SetEase(m_Ease);
        }
        else if (m_AnimType == 4)
        {
            m_Tweener = m_Role.transform.DOLocalMove(m_EndPos, m_Duration).SetEase(m_Ease);
        }

        m_Tweener.OnComplete(Complete);
    }

    protected override void OnResume()
    {
        m_Tweener.Restart();
    }
}