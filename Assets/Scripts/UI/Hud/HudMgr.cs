using WuWuFramework;
using WuWuFramework.UI;
using System.Collections.Generic;
using UnityEngine;

public class HudMgr : BaseMgr<HudMgr>
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

    public Queue<HudArg> hudArgs
    {
        get
        {
            return m_HudArgs;
        }
    }
    
    protected override void OnAwake()
    {
        m_HudArgs = new Queue<HudArg>();
    }

    protected override void OnUpdate()
    {
        
    }

    protected override void OnShutdown()
    {
        base.OnShutdown();
        GameEntry.uiMgr.Close<HudView>();
        m_HudArgs.Clear();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_HudArgs = null;
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
    
    private void ShowHud()
    {
        GameEntry.uiMgr.Open<HudView>();
    }

    private Queue<HudArg> m_HudArgs = null;
}
