using GameFrameWork;
using GameFrameWork.Utils;
using UnityEngine;

public class Barrel : BaseAvatar, ICanBeHit
{
    private BarrelData m_BarrelData;
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

    protected override void OnInit()
    {
        base.OnInit();
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

    public void HurtState(HurtStateArg arg)
    {
        entityAttribute.SubHealth(arg.attackValue);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Hurt));

        if (isDead)
        {
            SetTrigger(AnimName.Dead);
            ChangeState<BarrelDead>(arg);
            SceneEntityMgr.instance.CreateSceneItem(m_BarrelData.itemId, mapPos);
        }

        ReferencePool.Release(arg);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckThrow(collision.gameObject);
        CheckStrike(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
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
                GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Barrel));
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

    protected override void OnDrop()
    {
        SetTrigger(AnimName.Drop);
        ChangeState<BarrelDrop>();
        base.OnDrop();
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
            HurtStateArg hurtArg = HurtStateArg.Create();
            hurtArg.attackerDir = dir;
            hurtArg.attackForce = SkillUtil.GetSmoonForce(dir);
            hurtArg.isSwoon = true;
            player.HurtState(hurtArg);
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

        HurtStateArg hurtArg = HurtStateArg.Create();
        hurtArg.attackerDir = -role.dir;
        hurtArg.attackForce = SkillUtil.GetSmoonForce();
        hurtArg.attackerPos = pos;
        hurtArg.isSwoon = true;
        hurtArg.attackerId = entityID;
        hurtArg.attackValue = 1;
        hurtArg.hurtAnim = string.Empty;
        hurtArg.isGroundHurt = false;

        HurtState(hurtArg);
    }
}
