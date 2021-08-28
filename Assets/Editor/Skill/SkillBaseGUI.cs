using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillBaseGUI:SkillGUI
{
    public SkillBaseGUI(SkillConfigData skillConfigData) : base(skillConfigData) 
    {
        m_ListKey = new List<GameFrameWork.Input.KeyType>();
    }

    protected override void OnUpdateData()
    {
        m_ListKey.Clear();
        m_CurrId = m_CurrData.Id;
        m_CurrName = SkillHelper.CurrShowName;
        m_CurrLevel = m_CurrData.Level;
        m_CurrAnimName = m_CurrData.AnimationName;
        m_CurrHurtSound = m_CurrData.HurtSound;
        m_EnternalTriggerTime = m_CurrData.EnternalTiggerTime;
        m_AnimSpeed = m_CurrData.AnimSpeed;
        m_AnimTime = m_CurrData.AnimTime;
        m_Exp = m_CurrData.EXP;
        m_ListKey.AddRange(m_CurrData.Key.Keys);
    }

    protected override void OnGUI()
    {
        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrName = EditorGUILayout.TextField("名称", m_CurrName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.Name = m_CurrName;
                SkillHelper.SetShowNames();
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrId = EditorGUILayout.IntField("Id", m_CurrId);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.Id = m_CurrId;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrLevel = EditorGUILayout.IntField("Level", m_CurrLevel);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.Level = m_CurrLevel;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrAnimName = EditorGUILayout.TextField("AnimName", m_CurrAnimName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.AnimationName = m_CurrAnimName;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrHurtSound = EditorGUILayout.TextField("HurtSound", m_CurrAnimName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.HurtSound = m_CurrHurtSound;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_EnternalTriggerTime = EditorGUILayout.FloatField("EnternalTriggerTime", m_EnternalTriggerTime);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.EnternalTiggerTime = m_EnternalTriggerTime;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_AnimSpeed = EditorGUILayout.FloatField("AnimSpeed", m_AnimSpeed);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.AnimSpeed = m_AnimSpeed;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_AnimTime = EditorGUILayout.IntField("AnimTime", m_AnimTime);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.AnimTime = m_AnimTime;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_Exp = EditorGUILayout.IntField("EXP", m_Exp);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrData.EXP = m_Exp;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            m_CurrData.Type = (SkillConfigData.SkillType)EditorGUILayout.EnumPopup("SkillType", m_CurrData.Type);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            m_CurrData.DeployerType = (SkillConfigData.SkillDeployerType)EditorGUILayout.EnumPopup("SkillDeployerType", m_CurrData.DeployerType);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            m_CurrData.TriggerType = (SkillConfigData.SkillTriggerType)EditorGUILayout.EnumPopup("SkillTriggerType", m_CurrData.TriggerType);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            m_CurrData.IsInEffectPlaySound = EditorGUILayout.Toggle("EffectPlaySound", m_CurrData.IsInEffectPlaySound);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            m_CurrData.CanChangeDir = EditorGUILayout.Toggle("CanChangeDir", m_CurrData.CanChangeDir);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            m_CurrData.CanMove = EditorGUILayout.Toggle("CanMove", m_CurrData.CanMove);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("SkillKey");
            m_CurrData.Key.AddTrigger = EditorGUILayout.Toggle("AddTrigger", m_CurrData.Key.AddTrigger);

            for (int i = 0; i < m_CurrData.Key.Keys.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                m_CurrData.Key.Keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(m_CurrData.Key.Keys[i]);
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    m_ListKey.RemoveAt(i);
                    m_CurrData.Key.Keys = m_ListKey.ToArray();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("增加按键"))
            {
                m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                m_CurrData.Key.Keys = m_ListKey.ToArray();
                return;
            }

            EditorGUILayout.EndVertical();
        });
    }

    private int m_CurrId = 0;
    private int m_CurrLevel = 0;
    private string m_CurrName = string.Empty;
    private string m_CurrAnimName = string.Empty;
    private string m_CurrHurtSound = string.Empty;
    private float m_EnternalTriggerTime = 0;
    private float m_AnimSpeed = 0;
    private int m_AnimTime = 0;
    private int m_Exp = 0;
    private List<GameFrameWork.Input.KeyType> m_ListKey = null;
}
