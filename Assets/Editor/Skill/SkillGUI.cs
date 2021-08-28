using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGUI
{
    public SkillGUI(SkillConfigData skillConfigData)
    {
        m_CurrData = skillConfigData;
    }

    public void UpdateData(SkillConfigData skillConfigData)
    {
        m_CurrData = skillConfigData;
        OnUpdateData();
    }

    public void Draw()
    {
        OnGUI();
    }

    protected virtual void OnUpdateData() { }
    protected virtual void OnGUI() { }

    protected void ShowNotification(string content)
    {

    }

    protected SkillConfigData m_CurrData = null;
}
