using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillBaseGUI:SkillGUI
{
    public SkillBaseGUI(EditorWindow window) : base(window)
    {
        m_ListKey = new List<GameFrameWork.Input.KeyType>();
    }

    protected override void OnUpdateData()
    {
        m_ListKey.Clear();
        m_CurrId = SkillHelper.CurrConfigData.Id;
        m_CurrName = SkillHelper.CurrShowName;
        m_CurrLevel = SkillHelper.CurrConfigData.Level;
        m_CurrAnimName = SkillHelper.CurrConfigData.AnimationName;
        m_CurrHurtSound = SkillHelper.CurrConfigData.HurtSound;
        m_EnternalTriggerTime = SkillHelper.CurrConfigData.EnternalTiggerTime;
        m_AnimSpeed = SkillHelper.CurrConfigData.AnimSpeed;
        m_AnimTime = SkillHelper.CurrConfigData.AnimTime;
        m_Exp = SkillHelper.CurrConfigData.EXP;
        m_ListKey.AddRange(SkillHelper.CurrConfigData.Key.Keys);
    }

    protected override void OnGUI()
    {
        EditorGUILayout.Space(10f);

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(SkillHelper.CurrShowName, SkillHelper.IndexLabelStyle);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("x"))
        {
            SkillHelper.RemoveData();
        }
        EditorGUILayout.EndHorizontal();

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrName = EditorGUILayout.TextField("名称", m_CurrName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillHelper.CurrConfigData.Name = m_CurrName;
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
                SkillHelper.CurrConfigData.Id = m_CurrId;
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
                SkillHelper.CurrConfigData.Level = m_CurrLevel;
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
                SkillHelper.CurrConfigData.AnimationName = m_CurrAnimName;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrHurtSound = EditorGUILayout.TextField("HurtSound", m_CurrHurtSound);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillHelper.CurrConfigData.HurtSound = m_CurrHurtSound;
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
                SkillHelper.CurrConfigData.EnternalTiggerTime = m_EnternalTriggerTime;
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
                SkillHelper.CurrConfigData.AnimSpeed = m_AnimSpeed;
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
                SkillHelper.CurrConfigData.AnimTime = m_AnimTime;
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
                SkillHelper.CurrConfigData.EXP = m_Exp;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            SkillHelper.CurrConfigData.Type = (SkillConfigData.SkillType)EditorGUILayout.EnumPopup("SkillType", SkillHelper.CurrConfigData.Type);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            SkillHelper.CurrConfigData.DeployerType = (SkillConfigData.SkillDeployerType)EditorGUILayout.EnumPopup("SkillDeployerType", SkillHelper.CurrConfigData.DeployerType);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            SkillHelper.CurrConfigData.TriggerType = (SkillConfigData.SkillTriggerType)EditorGUILayout.EnumPopup("SkillTriggerType", SkillHelper.CurrConfigData.TriggerType);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            SkillHelper.CurrConfigData.IsInEffectPlaySound = EditorGUILayout.Toggle("EffectPlaySound", SkillHelper.CurrConfigData.IsInEffectPlaySound);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            SkillHelper.CurrConfigData.CanChangeDir = EditorGUILayout.Toggle("CanChangeDir", SkillHelper.CurrConfigData.CanChangeDir);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            SkillHelper.CurrConfigData.CanMove = EditorGUILayout.Toggle("CanMove", SkillHelper.CurrConfigData.CanMove);
        });

        GameFrameWork.Editor.EditorUtility.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("SkillKey");
            SkillHelper.CurrConfigData.Key.AddTrigger = EditorGUILayout.Toggle("AddTrigger", SkillHelper.CurrConfigData.Key.AddTrigger);

            for (int i = 0; i < SkillHelper.CurrConfigData.Key.Keys.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SkillHelper.CurrConfigData.Key.Keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(SkillHelper.CurrConfigData.Key.Keys[i]);
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    m_ListKey.RemoveAt(i);
                    SkillHelper.CurrConfigData.Key.Keys = m_ListKey.ToArray();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("增加按键"))
            {
                m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                SkillHelper.CurrConfigData.Key.Keys = m_ListKey.ToArray();
                return;
            }

            EditorGUILayout.EndVertical();
        });

        EditorGUILayout.EndScrollView();
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
    private Vector2 m_ScrollPos = Vector2.zero;
}