using DragonBones;
using GameFrameWork;
using GameFrameWork.Audio;
using UnityEngine;

public class Weapon : BaseSceneItem
{
    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_WeaponData = data as SceneItemData;
    }

    public void Drop(float attackerDir)
    {
        if (!m_WeaponData.canDrop)
        {
            return;
        }
    
        SetActive(true);
        SetPosXY(m_Owner.pos.x, m_Owner.pos.y);
        AddForce(40f * attackerDir, 150f);
        PlayAnimation(AnimName.Drop);
        m_Owner = null;
    }

    public override void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
        gameObject.SetActive(false);
        SceneEntityMgr.instance.ReleaseSceneItem(this);
        AudioMgr.instance.PlaySE(ResDefine.AudioClipPath, SoundName.Bonus);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_Animator.animation.isPlaying)
        {
            int frameCount = (int)m_Animator.animation.animations[m_CurrAnimName].frameCount;
            float duration = m_Animator.animation.animations[m_CurrAnimName].duration;
            int frameIndex = (int)(m_Animator.animation.GetState(m_CurrAnimName).currentTime * frameCount / duration);

            SetTrigger(m_CurrAnimName, frameIndex);
        }
    }

    protected override void OnResComplete(GameObject go, object[] param)
    {
        base.OnResComplete(go, param);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_HitTrigger = m_ResGO.GetComponent<HitTrigger>();
        PlayAnimation(AnimName.Idle);
        SetPos2(m_Pos);
        ResetRigidbody();
    }

    protected override void OnGround()
    {
        SetPos2(m_Pos);

        if (m_EntityAttribute.IsDie())
        {
            Release();
        }
        else
        {
            PlayAnimation(AnimName.Idle);
        }
    }

    private void PlayAnimation(string animName)
    {
        if (m_Animator == null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return;
        }

        if (IsAnimation(animName))
        {
            if (!m_Animator.animation.isCompleted)
            {
                return;
            }

            m_CurrAnimName = string.Empty;
        }

        SetTrigger(animName);

        m_CurrAnimName = animName;
        m_Animator.animation.Play(animName, 1);
    }

    private bool IsAnimation(string animName)
    {
        if (m_Animator == null)
        {
            Log.LogError(name, "[Animator] 组件不存在");
            return false;
        }

        bool result = m_CurrAnimName.Equals(animName);

        if (m_Animator.animation.isCompleted)
        {
            m_CurrAnimName = string.Empty;
        }

        return result;
    }

    private void SetTrigger(string animName,int frameIndex = 0)
    {
        if (m_HitTrigger == null)
        {
            return;
        }

        TriggerData triggerData = m_HitTrigger.GetTriggerData(animName);

        if (triggerData != null)
        {
            SetCollider(triggerData.offestList[frameIndex], triggerData.sizeList[frameIndex]);
        }
    }

    public override void Release()
    {
        base.Release();
        m_WeaponData = null;
        m_HitTrigger = null;
        m_Animator = null;
    }

    private string m_CurrAnimName = string.Empty;
    private HitTrigger m_HitTrigger = null;
    private UnityArmatureComponent m_Animator = null;
    private SceneItemData m_WeaponData = null;
}
