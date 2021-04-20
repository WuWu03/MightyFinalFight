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

    public override void InitInfo(BaseSceneObjectInfo data)
    {
        base.InitInfo(data);
        m_WeaponData = data as SceneItemInfo;
    }

    public override void SubHealth(int value)
    {
        base.SubHealth(value);
    }

    public void Drop()
    {
        if (!m_WeaponData.CanDrop) return;
        gameObject.SetActive(true);
        SetPos2(m_Owner.Pos.x, m_Owner.Bound.yMin + Bound.height / 2);
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody.AddForce(new Vector2(40 * -m_Owner.Dir, 150));
        m_Animator.animation.Play(AnimName.Drop, 0);
        m_Owner = null;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        gameObject.SetActive(false);
        SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/Bonus");
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        SetCollider(m_WeaponData.TriggerOffest, m_WeaponData.TriggerSize);
        SetPos(m_Pos);
        m_Collider.isTrigger = true;
        m_Collider.enabled = true;
        m_Rigidbody.gravityScale = 1.0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Animator.animation.Play(AnimName.Idle, 0);
    }

    protected override void CheckGround()
    {
        base.CheckGround();
        if (!IsInGround || !ResComplete) return;
        m_Rigidbody.velocity = Vector2.zero;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;

        SetPos(m_Pos);

        if(m_Health <= 0)
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
    private SceneItemInfo m_WeaponData = null;
}
