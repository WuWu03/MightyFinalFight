using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillBaseGUI : SkillGUI
{
    public SkillBaseGUI(EditorWindow window) : base(window)
    {
        m_ListKey = new List<GameFrameWork.Input.KeyType>();
    }

    protected override void OnUpdateData()
    {
        m_ListKey.Clear();
        m_CurrId = SkillEditorHelper.CurrConfigData.Id;
        m_CurrName = SkillEditorHelper.CurrShowName;
        m_CurrLevel = SkillEditorHelper.CurrConfigData.Level;
        m_CurrAnimName = SkillEditorHelper.CurrConfigData.AnimationName;
        m_CurrHurtSound = SkillEditorHelper.CurrConfigData.HurtSound;
        m_EnternalTriggerTime = SkillEditorHelper.CurrConfigData.EnternalTiggerTime;
        m_AnimSpeed = SkillEditorHelper.CurrConfigData.AnimSpeed;
        m_AnimTime = SkillEditorHelper.CurrConfigData.AnimTime;
        m_Exp = SkillEditorHelper.CurrConfigData.EXP;

        if (SkillEditorHelper.CurrConfigData.Key.Keys != null)
        {
            m_ListKey.AddRange(SkillEditorHelper.CurrConfigData.Key.Keys);
        }
    }

    protected override void OnGUI()
    {
        EditorGUILayout.Space(10f);

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        GUILayout.Label(SkillEditorHelper.CurrShowName, SkillEditorHelper.IndexLabelStyle);

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrName = EditorGUILayout.TextField("名称", m_CurrName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.Name = m_CurrName;
                SkillEditorHelper.SetShowNames();
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrId = EditorGUILayout.IntField("Id", m_CurrId);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.Id = m_CurrId;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrLevel = EditorGUILayout.IntField("Level", m_CurrLevel);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.Level = m_CurrLevel;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrAnimName = EditorGUILayout.TextField("AnimName", m_CurrAnimName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.AnimationName = m_CurrAnimName;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrHurtSound = EditorGUILayout.TextField("HurtSound", m_CurrHurtSound);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.HurtSound = m_CurrHurtSound;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_EnternalTriggerTime = EditorGUILayout.FloatField("EnternalTriggerTime", m_EnternalTriggerTime);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.EnternalTiggerTime = m_EnternalTriggerTime;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_AnimSpeed = EditorGUILayout.FloatField("AnimSpeed", m_AnimSpeed);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.AnimSpeed = m_AnimSpeed;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_AnimTime = EditorGUILayout.IntField("AnimTime", m_AnimTime);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.AnimTime = m_AnimTime;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_Exp = EditorGUILayout.IntField("EXP", m_Exp);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.CurrConfigData.EXP = m_Exp;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.CurrConfigData.Type = (SkillConfigData.SkillType)EditorGUILayout.EnumPopup("SkillType", SkillEditorHelper.CurrConfigData.Type);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.CurrConfigData.DeployerType = (SkillConfigData.SkillDeployerType)EditorGUILayout.EnumPopup("SkillDeployerType", SkillEditorHelper.CurrConfigData.DeployerType);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.CurrConfigData.TriggerType = (SkillConfigData.SkillTriggerType)EditorGUILayout.EnumPopup("SkillTriggerType", SkillEditorHelper.CurrConfigData.TriggerType);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.CurrConfigData.IsInEffectPlaySound = EditorGUILayout.Toggle("EffectPlaySound", SkillEditorHelper.CurrConfigData.IsInEffectPlaySound);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.CurrConfigData.CanChangeDir = EditorGUILayout.Toggle("CanChangeDir", SkillEditorHelper.CurrConfigData.CanChangeDir);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.CurrConfigData.CanMove = EditorGUILayout.Toggle("CanMove", SkillEditorHelper.CurrConfigData.CanMove);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("SkillKey");
            SkillEditorHelper.CurrConfigData.Key.AddTrigger = EditorGUILayout.Toggle("AddTrigger", SkillEditorHelper.CurrConfigData.Key.AddTrigger);

            for (int i = 0; i < SkillEditorHelper.CurrConfigData.Key.Keys.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SkillEditorHelper.CurrConfigData.Key.Keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(SkillEditorHelper.CurrConfigData.Key.Keys[i]);
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    m_ListKey.RemoveAt(i);
                    SkillEditorHelper.CurrConfigData.Key.Keys = m_ListKey.ToArray();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("增加按键"))
            {
                m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                SkillEditorHelper.CurrConfigData.Key.Keys = m_ListKey.ToArray();
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