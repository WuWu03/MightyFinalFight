using DragonBones;
using FrameWork;
using UnityEditor.Timeline;
using UnityEngine;

public class Weapon : BaseSceneItem
{
    public override void InitData(BaseSceneObjectData data)
    {
        base.InitData(data);
        m_WeaponData = data as ItemData;
    }

    public override void SubHealth(int value)
    {
        base.SubHealth(value);
    }

    public void Drop()
    {
        m_ResGO.SetActive(true);
        transform.SetParent(null, false);
        m_Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        m_Rigidbody.AddForce(new Vector2(40 * -m_Owner.Dir, 150));
        m_Animator.animation.Play(AnimName.Drop, 0);
        m_Owner = null;
    }

    public override void SetOwner(BaseRole owner)
    {
        base.SetOwner(owner);
        transform.SetParent(owner.transform, false);
        m_ResGO.gameObject.SetActive(false);
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        m_Animator = go.GetComponent<UnityArmatureComponent>();
        SetCollider(m_WeaponData.TriggerOffest, m_WeaponData.TriggerSize);
        m_Collider.isTrigger = true;
        m_Collider.enabled = true;
        m_Rigidbody.gravityScale = 1.0f;
        m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_Animator.animation.Play(AnimName.Idle, 0);
    }

    protected override void Update()
    {
        base.Update();

        if (m_Rigidbody.bodyType != RigidbodyType2D.Dynamic) return;

        UpdatePos2(transform.localPosition.x, Pos.y);

        if (IsFloat)
        {
            return;
        }

        CheckGround();
    }

    private void CheckGround()
    {
        if (!IsInGround) return;
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
    private ItemData m_WeaponData = null;
}
