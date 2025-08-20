using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Utils;
using UnityEngine;

public class Barrel : BaseAvatar, ICanBeHit
{
    public bool canBeHit
    {
        get
        {
            return !isDead && isAssetLoadComplete;
        }
    }

    public bool isBeCatch
    {
        get
        {
            return false;
        }
    }


    public bool isDead
    {
        get
        {
            return entityAttribute.health <= 0;
        }
    }

    public bool isSwoon
    {
        get
        {
            return false;
        }
    }

    public BarrelData barrelData
    {
        get
        {
            return m_BarrelData;
        }
    }

    public bool isBeThrow
    {
        get
        {
            return false;
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<BarrelIdle>();
        AddState<BarrelMove>();
        AddState<BarrelDrop>();
        AddState<BarrelDead>();
    }

    public override void SetData(BaseSceneObjectData data)
    {
        base.SetData(data);
        m_BarrelData = data as BarrelData;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        m_BarrelData = null;
    }

    public bool IsHurtWillDie(int attackValue)
    {
        return entityAttribute.health - attackValue <= 0;
    }

    public void OnHurtMsg(HurtStateData data)
    {
        entityAttribute.SubHealth(data.attackValue);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Hurt));

        if (isDead)
        {
            SetTrigger(AnimName.Dead);
            ChangeState<BarrelDead>(data);
            SceneEntityMgr.instance.CreateSceneItem(m_BarrelData.itemId, mapPos);
        }

        ReferencePool.Release(data);
    }

    public void SetIsBeCatch(bool value) { }

    public void SetIsBeThrow(bool value) { }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        if (m_BarrelData != null)
        {
            bool isOut = m_BarrelData.dir > 0 ? IsOutVersionXRight(pos.x) : IsOutVersionXLeft(pos.x);

            if (isOut)
            {
                Release();
                return;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        CheckThrow(collision.gameObject);
        CheckStrike(collision.gameObject);
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        CheckStrike(collision.gameObject);
    }

    //protected override void OnTriggerExit2D(Collider2D collision)
    //{
    //    CheckStrike(collision.gameObject);
    //}

    protected override void OnLoadAssetComplete(GameObject go, object arg)
    {
        base.OnLoadAssetComplete(go, arg);
        PlayAnimation(AnimName.Idle, 0);

        if (!m_BarrelData.isFloat)
        {
            if (m_BarrelData.moveSpeed >= 0)
            {
                SetTrigger(AnimName.Move);
                fsm.Start<BarrelMove>();
                AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Barrel));
            }
            else
            {
                SetTrigger(AnimName.Idle);
                fsm.Start<BarrelIdle>();
            }
        }
        else
        {
            UpdatePosY(m_BarrelData.groundY / 100f);
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    protected override void OnGround()
    {
        SetTrigger(AnimName.Drop);
        ChangeState<BarrelDrop>();
    }

    private void CheckStrike(GameObject go)
    {
        if (!isAssetLoadComplete || m_BarrelData.moveSpeed <= 0 || isDead)
        {
            return;
        }

        BaseHero player = PlayerMgr.instance.player;

        if (player == null || player.gameObject != go)
        {
            return;
        }
          
        bool isInRange = false;

        if (SkillUtil.IsRectangleCollide(player.bound, bound) && player.pos.y >= pos.y)
        {
            Vector2 bsoLeftTop = new Vector2(bound.xMin, bound.yMax) - bound.center;
            float selectorAngle = Vector2.Angle(Vector2.left, bsoLeftTop.normalized);

            Vector2 target = (player.pos - pos).normalized;
            Vector2 normal = dir >= 0 ? Vector2.right : Vector2.left - Vector2.zero;
            float angle = Vector2.Angle(target, normal);

            if (angle <= selectorAngle / 2)
            {
                isInRange = true;
            }
        }

        if (isInRange)
        {
            HurtStateData hurtData = HurtStateData.Create();
            hurtData.attackerDir = dir;
            hurtData.attackForce = SkillUtil.GetSmoonForce(dir);
            hurtData.isSwoon = true;
            player.OnHurtMsg(hurtData);
        }
    }

    private void CheckThrow(GameObject go)
    {
        if (!isAssetLoadComplete || isDead)
        {
            return;
        }

        BaseRole role = go.GetComponent<BaseRole>();

        if (role == null || role.objectType != ObjectType.Enemy || !role.isBeThrow || !role.canBeHit)
        {
            return;
        }

        if (Mathf.Abs(role.pos.y - pos.y) > 0.1f)
        {
            return;
        }

        HurtStateData hurtData = HurtStateData.Create();
        hurtData.attackerDir = -role.dir;
        hurtData.attackForce = SkillUtil.GetSmoonForce();
        hurtData.attackerPos = pos;
        hurtData.isSwoon = true;
        hurtData.attackerId = id;
        hurtData.attackValue = 1;
        hurtData.hurtAnim = string.Empty;
        hurtData.isGroundHurt = false;

        OnHurtMsg(hurtData);
    }

    private BarrelData m_BarrelData = null;
}
