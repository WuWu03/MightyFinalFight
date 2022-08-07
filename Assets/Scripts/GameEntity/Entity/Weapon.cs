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
        if (!m_WeaponData.CanDrop) return;
    
        SetActive(true);
        SetPosXY(m_Owner.Pos.x, m_Owner.Pos.y);
        AddForce(40f * attackerDir, 150f);
        SetTrigger(AnimName.Drop);

        m_Animator.animation.Play(AnimName.Drop, 0);
        m_Owner = null;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        gameObject.SetActive(false);
        SoundMgr.Ins.PlaySound(ResDefine.AudioClipPath, "Sound/Bonus");
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        m_Animator.animation.Play(AnimName.Idle, 0);
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
            m_Animator.animation.Play(AnimName.Idle, 0);
        }
    }

    public override void Release()
    {
        base.Release();
        m_WeaponData = null;
    }

    private UnityArmatureComponent m_Animator = null;
    private SceneItemData m_WeaponData = null;
}
