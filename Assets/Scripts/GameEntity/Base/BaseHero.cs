using FrameWork.Camera;
using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseRole
{
    public override bool CanMove
    {
        get
        {
            return base.CanMove && !HasCatch();
        }
    }

    public override bool CanSkill
    {
        get
        {
            return base.CanSkill && !HasCatch();
        }
    }

    public override bool CanAttack
    {
        get
        {
            return base.CanAttack || HasCatch();
        }
    }


    public override bool CanJump
    {
        get
        {
            return base.CanJump || HasCatch();
        }
    }

    public override bool CanChangeDefaultState
    {
        get
        {
            return base.CanChangeDefaultState || HasCatch();
        }
    }

    public override bool CanBeHit
    {
        get
        {
            return base.CanBeHit && !m_IsRebirthState && !IsFloat;
        }
    }

    public bool IsCatch
    {
        get
        {
            return HasCatch();
        }
    }

    public override void Init(int id, string name)
    {
        base.Init(id, name);
        AddState<HeroRebirth>();
        AddState<HeroCatch>();
        m_DicAttacker = new Dictionary<int, int>();
        m_ListCatchTarget = new List<ICanBeHit>();
    }

    public override void Release()
    {
        base.Release();
        m_DicAttacker.Clear();
        m_ListCatchTarget.Clear();
        m_ListCatchTarget = null;
        m_DicAttacker = null;
    }

    protected override void Update()
    {
        base.Update();
        CheckCatch();
        CheckRebirthState();

        if (m_Rigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            if (IsOutVersionX(transform.localPosition.x) && Mathf.Abs(m_Rigidbody.velocity.x) > 0)
            {
                m_Rigidbody.velocity = new Vector2(0, m_Rigidbody.velocity.y);
            }
        }

        if (m_HitTime < 0) return;

        if (Time.time - m_HitTime > 1.5f)
        {
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
    }

    protected override void OnResComplete(GameObject go)
    {
        base.OnResComplete(go);
        go.transform.Find("slot1").GetComponent<Renderer>().enabled = true;
        go.transform.Find("slot2").GetComponent<Renderer>().enabled = false;
    }
    public override List<ICanBeHit> OnHitStart()
    {
        if (m_ListCatchTarget.Count < 1) return null;
        return m_ListCatchTarget;
    }

    public override void OnHitEnd(SkillData skillData,bool isHurtTarget)
    {
        base.OnHitEnd(skillData, isHurtTarget);

        if (m_ListCatchTarget.Count < 1 || !isHurtTarget || m_CatchAttackCount >= 3) return;

        if (skillData.Type == SkillData.SkillType.SkillAttack)//捕捉状态下技能攻击不进行次数累积
        {
            ResetCatch(false);
            return;
        }

        m_CatchAttackCount++;

        if (m_CatchAttackCount >= 3)
        {
            m_ListCatchTarget[0].OnHurtMsg(new HurtData()
            {
                AttackerDir = m_Dir,
                AttackValue = 0,
                IsSwoon = true,
                AttackForce = new Vector2(40f * m_Dir, 150f),
            });
        }
    }

    public override void OnAttackMsg(AttackData data,bool isForceJumpAttack = false)
    {
        if(m_ListCatchTarget != null)
        {
            m_CatchStamp = Time.time;
        }

        if (m_CatchAttackCount >= 3) return;
        base.OnAttackMsg(data, isForceJumpAttack);
    }

    public override void OnJumpMsg(JumpData data)
    {
        if (HasCatch())
            ResetCatch();
        base.OnJumpMsg(data);
    }

    public override void OnHurtMsg(HurtData data)
    {
        if (HasCatch())
            ResetCatch(false);
        if (data.IsSwoon)
        {
            m_DicAttacker.Clear();
            m_HitTime = -1f;
        }
        else
        {
            int hitTime = 0;
            if (!m_DicAttacker.TryGetValue(data.AttackerID, out hitTime))
            {
                m_DicAttacker.Add(data.AttackerID, hitTime);
            }

            hitTime++;
            m_DicAttacker[data.AttackerID] = hitTime;

            if (hitTime >= 3)
            {
                data.AttackForce = new Vector2(40 * data.AttackerDir, 120);
                data.IsSwoon = true;
                data.HurtSound = "OnBlow";
                m_DicAttacker.Clear();
            }
            m_HitTime = Time.time;
        }

        base.OnHurtMsg(data);
    }

    public override void AddHealth(int value)
    {
        base.AddHealth(value);
        UIMgr.Ins.GetPanel<MainPanelCtrl>().SetPlayerHP(m_Health, m_MaxHealth);
    }

    public override void SubHealth(int value)
    {
        base.SubHealth(value);
        UIMgr.Ins.GetPanel<MainPanelCtrl>().SetPlayerHP(m_Health, m_MaxHealth);
    }

    public override void SetPos(Vector2 pos)
    {
        if (IsAnyState(typeof(RoleMove)))
        {
            bool isMapCanMove = StageMgr.Ins.CanMovePos2(pos);
            if (!isMapCanMove)
            {
                CameraMgr.Ins.EndFollow();
            }
            else
            {
                CameraMgr.Ins.StartFollow();
            }

            if (!CanMove || !isMapCanMove || IsOutVersionX(pos.x)) return;
        }

        base.SetPos(pos);
    }

    public virtual void OnRebirthMsg(Vector2 rebirthPos)
    {
        ChangeState<HeroRebirth>();
        m_ResGO.transform.Find("slot1").GetComponent<Renderer>().enabled = false;
        m_ResGO.transform.Find("slot2").GetComponent<Renderer>().enabled = true;
        m_IsRebirthState = true;
        m_RebirthStateTimer = Time.time;
        UIMgr.Ins.GetPanel<MainPanelCtrl>().SetPlayerHP(m_Health, m_MaxHealth);
    }

    protected virtual void CheckCatch()
    {
        if (m_CatchAttackCount >= 3 && IsPlayComplete())
        {
            ResetCatch(true);
            return;
        }

        if (m_ListCatchTarget.Count < 1)
        {
            if (!IsAnyState(typeof(RoleMove)) || m_TriggerTargets.Targets.Count < 1) return;

            for (int i = 0; i < m_TriggerTargets.Targets.Count; i++)
            {
                ICanBeHit temp = m_TriggerTargets.Targets[i].GetComponent<ICanBeHit>();
                if (temp == null || !temp.CanBeHit || !(temp is BaseAvatar)) continue;

                BaseSceneObject targetObj = temp as BaseSceneObject;
                bool isInRange = Mathf.Abs(targetObj.Pos.y - m_Pos.y) <= 0.03f &&
                                 Mathf.Abs(targetObj.Pos.x - m_Pos.x) <= 0.17f &&
                                    (targetObj.Pos.x - m_Pos.x) * m_Dir > 0;
                if (isInRange)
                {
                    targetObj.SetDir(m_Dir * -1);
                    targetObj.SetPos2(m_Pos.x + 0.17f * m_Dir, m_Pos.y);
                    temp.SetCatch(true);
                    ChangeState<HeroCatch>();
                    SetDefaultState<HeroCatch>();
                    m_ListCatchTarget.Add(temp);
                    break;
                }
            }

            if(m_ListCatchTarget.Count > 0)
            {
                m_CatchStamp = Time.time;
            }

            return;
        }

        if (Time.time - m_CatchStamp >= m_CatchTime || m_ListCatchTarget[0].IsDead)
        {
            ResetCatch();
            return;
        }
    }

    private void CheckRebirthState()
    {
        if (!m_IsRebirthState) return;

        if(Time.time - m_RebirthStateTimer >= m_RebirthStateTime)
        {
            m_RebirthStateTimer = 0;
            m_IsRebirthState = false;
            m_ResGO.transform.Find("slot1").GetComponent<Renderer>().enabled = true;
            m_ResGO.transform.Find("slot2").GetComponent<Renderer>().enabled = false;
        }
    }

    public void ResetCatch(bool changeState = true)
    {
        m_ListCatchTarget[0].SetCatch(false);
        m_ListCatchTarget.Clear();
        m_CatchStamp = 0f;
        m_CatchAttackCount = 0;

        SetDefaultState<RoleIdle>();

        if (changeState)
        {
            ChangeState<RoleIdle>();
        }
    }

    private bool HasCatch()
    {
        return m_ListCatchTarget != null && m_ListCatchTarget.Count > 0;
    }

    protected float m_CatchTime = 2;
    protected bool m_IsRebirthState = false;

    private float m_RebirthStateTimer = 0;
    private float m_RebirthStateTime = 3.0f;
    private float m_CatchStamp = 0f;
    private float m_HitTime = -1f;
    private int m_CatchAttackCount = 0;
    private List<ICanBeHit> m_ListCatchTarget = null;
    private Dictionary<int, int> m_DicAttacker = null;
}