using DragonBones;
using GameFrameWork.Sound;
using UnityEngine;

public class Weapon : BaseSceneItem
{
    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_WeaponData = data as SceneItemData;
    }

    //public override void SubHealth(int value)
    //{
    //    m_EntityAttribute.SubHealth(value);
    //}

    public void Drop(float attackerDir)
    {
        if (!m_WeaponData.canDrop)
        {
            return;
        }
    
        SetActive(true);
        SetPosXY(m_Owner.pos.x, m_Owner.pos.y);
        AddForce(40f * attackerDir, 150f);
        SetTrigger(AnimName.Drop);
        PlayAnimation(AnimName.Drop);
        m_Owner = null;
    }

    public override void SetOwner(BaseRole owner)
    {
        m_Owner = owner;
        gameObject.SetActive(false);
        SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, "Sound/Bonus");
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_HitTrigger = m_ResGO.GetComponent<HitTrigger>();
        PlayAnimation(AnimName.Idle);
        SetPos2(m_Pos);
        ResetRigidbody();
        SetTrigger(AnimName.Idle);
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
        if(m_Animator == null)
        {
            return;
        }

        m_Animator.animation.Play(animName);
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

    private HitTrigger m_HitTrigger = null;
    private UnityArmatureComponent m_Animator = null;
    private SceneItemData m_WeaponData = null;
}
