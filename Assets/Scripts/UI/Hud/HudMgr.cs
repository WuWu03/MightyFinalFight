using System.Collections.Generic;
using UnityEngine;
using WuWuFramework;

public class HudMgr : Singleton<HudMgr>
{
    public enum DamageType
    {
        Player = 1,
        Enemy = 2,
    }

    public struct HudArg
    {
        public int value;
        public Vector3 pos;
        public DamageType damageType;
    }

    private Queue<HudArg> m_HudArgs;

    public Queue<HudArg> hudArgs
    {
        get
        {
            return m_HudArgs;
        }
    }

    public HudMgr()
    {
        m_HudArgs = new();
    }

    public void ShowEnemyDamage(int value, Vector3 pos)
    {
        m_HudArgs.Enqueue(new HudArg { value = value, pos = pos, damageType = DamageType.Enemy });
        ShowHud();
    }

    public void ShowPlayerDamage(int value, Vector3 pos)
    {
        m_HudArgs.Enqueue(new HudArg { value = value, pos = pos, damageType = DamageType.Player });
        ShowHud();
    }

    public override void Shutdown()
    {
        m_HudArgs.Clear();
    }

    private void ShowHud()
    {
        GameEntry.uiMgr.Open<HudView>();
    }
}
