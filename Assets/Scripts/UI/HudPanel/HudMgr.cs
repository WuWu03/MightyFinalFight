using GameFrameWork;
using GameFrameWork.UI;
using System.Collections.Generic;
using UnityEngine;

public class HudMgr : BaseMgr<HudMgr>
{
    public enum DamageType
    {
        Player = 1,
        Enemy = 2,
    }

    private struct HudArg
    {
        public int value;
        public Vector3 pos;
        public DamageType damageType;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        m_HudArgs = new Queue<HudArg>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_HUDView is not { isOpen: true })
        {
            return;
        }

        lock (m_HudArgs)
        {
            while (m_HudArgs.Count > 0)
            {
                HudArg arg = m_HudArgs.Dequeue();
                m_HUDView.ShowDamageText(arg.damageType, arg.value, arg.pos);
            }
        }
    }

    protected override void OnShutDown()
    {
        base.OnShutDown();
        UIMgr.instance.Close(UINames.HudPanel);
        lock (m_HudArgs)
        {
            m_HudArgs.Clear();
        }
    }

    protected override void OnDestory()
    {
        base.OnDestory();

        lock (m_HudArgs)
        {
            m_HudArgs.Clear();
            m_HudArgs = null;
        }
    }

    public void ShowEnemyDamage(int value, Vector3 pos)
    {
        lock (m_HudArgs)
        {
            m_HudArgs.Enqueue(new HudArg { value = value, pos = pos, damageType = DamageType.Enemy });
        }

        ShowHud();
    }

    public void ShowPlayerDamage(int value, Vector3 pos)
    {
        lock (m_HudArgs)
        {
            m_HudArgs.Enqueue(new HudArg { value = value, pos = pos, damageType = DamageType.Player });
        }

        ShowHud();
    }

    private void ShowHud()
    {
        m_HUDView ??= UIMgr.instance.Get(UINames.HudPanel) as HudView;
    }

    private Queue<HudArg> m_HudArgs = null;
    private HudView m_HUDView = null;
}
