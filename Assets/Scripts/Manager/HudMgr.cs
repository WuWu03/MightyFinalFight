using GameFrameWork;
using GameFrameWork.UI;
using System.Collections.Generic;
using UnityEngine;

public class HudMgr : BaseMgr<HudMgr>
{
    enum DamageType
    {
        Player = 1,
        Enemy = 2,
    }

    struct HudArg
    {
        public int value;
        public Vector3 pos;
        public DamageType damageType;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        m_QueueHudArgs = new Queue<HudArg>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_HudPanel == null || !m_HudPanel.isOpen)
        {
            return;
        }

        lock (m_QueueHudArgs)
        {
            while (m_QueueHudArgs.Count > 0)
            {
                HudArg arg = m_QueueHudArgs.Dequeue();

                if (arg.damageType == DamageType.Player)
                {
                    m_HudPanel.ShowPlayerDamage(arg.value, arg.pos);
                }
                else
                {
                    m_HudPanel.ShowEnemyDamage(arg.value, arg.pos);
                }
            }
        }
    }

    protected override void OnShutDown()
    {
        base.OnShutDown();
        UIMgr.instance.Close<HudPanel>();
        m_QueueHudArgs.Clear();
        m_QueueHudArgs = null;
    }

    public void ShowEnemyDamage(int value, Vector3 pos)
    {
        lock (m_QueueHudArgs)
        {
            m_QueueHudArgs.Enqueue(new HudArg { value = value, pos = pos, damageType = DamageType.Enemy });
        }

        ShowHud();
    }

    public void ShowPlayerDamage(int value, Vector3 pos)
    {
        lock (m_QueueHudArgs)
        {
            m_QueueHudArgs.Enqueue(new HudArg { value = value, pos = pos, damageType = DamageType.Enemy });
        }

        ShowHud();
    }

    private void ShowHud()
    {
        if (m_HudPanel == null)
        {
            m_HudPanel = UIMgr.instance.Open<HudPanel>();
        }
    }

    private Queue<HudArg> m_QueueHudArgs = null;
    private HudPanel m_HudPanel = null;
}
