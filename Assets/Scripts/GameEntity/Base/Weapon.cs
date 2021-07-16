using DragonBones;
using GameFrameWork.Sound;
using UnityEngine;

public class Weapon : BaseSceneItem
{
    public int WeaponHealth
    {
        get
        {
            return m_WeaponData.Health;
        }
    }


    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_WeaponData = data as SceneItemData;
    }

    public override void SubHealth(int value)
    {
        base.SubHealth(value);
    }

    public void Drop(float attackerDir)
    {
        if (!m_WeaponData.CanDrop) return;
    
        SetActive(true);
        SetPos2(m_Owner.Pos.x, m_Owner.Pos.y);

        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody.AddForce(new Vector2(40f * attackerDir, 150f));
        m_Animator.animation.Play(AnimName.Drop, 0);
        m_Owner = null;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        gameObject.SetActive(false);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/Bonus");
    }

    protected override void OnResComplete(GameObject go,object[] param)
    {
        base.OnResComplete(go, param);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        SetCollider(m_WeaponData.TriggerOffest, m_WeaponData.TriggerSize);
        SetPos(m_Pos);
        m_Collider.isTrigger = true;
        m_Collider.enabled = true;
        m_Rigidbody.gravityScale = 1.0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Animator.animation.Play(AnimName.Idle, 0);
    }

    protected override void OnGround()
    {
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;

        SetPos(m_Pos);

        if (m_Health <= 0)
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
