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
        m_CurrId = SkillEditorHelper.currConfigData.id;
        m_CurrName = SkillEditorHelper.currShowName;
        m_CurrLevel = SkillEditorHelper.currConfigData.Level;
        m_CurrAnimName = SkillEditorHelper.currConfigData.AnimationName;
        m_CurrHurtSound = SkillEditorHelper.currConfigData.HurtSound;
        m_EnternalTriggerTime = SkillEditorHelper.currConfigData.EnternalTiggerTime;
        m_AnimSpeed = SkillEditorHelper.currConfigData.AnimSpeed;
        m_AnimTime = SkillEditorHelper.currConfigData.AnimTime;
        m_Exp = SkillEditorHelper.currConfigData.EXP;

        if (SkillEditorHelper.currConfigData.Key.Keys != null)
        {
            m_ListKey.AddRange(SkillEditorHelper.currConfigData.Key.Keys);
        }
    }

    protected override void OnGUI()
    {
        EditorGUILayout.Space(10f);

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
        GUILayout.Label(SkillEditorHelper.currShowName, SkillEditorHelper.IndexLabelStyle);

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginHorizontal();
            m_CurrName = EditorGUILayout.TextField("名称", m_CurrName);

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                SkillEditorHelper.currConfigData.Name = m_CurrName;
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
                SkillEditorHelper.currConfigData.id = m_CurrId;
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
                SkillEditorHelper.currConfigData.Level = m_CurrLevel;
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
                SkillEditorHelper.currConfigData.AnimationName = m_CurrAnimName;
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
                SkillEditorHelper.currConfigData.HurtSound = m_CurrHurtSound;
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
                SkillEditorHelper.currConfigData.EnternalTiggerTime = m_EnternalTriggerTime;
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
                SkillEditorHelper.currConfigData.AnimSpeed = m_AnimSpeed;
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
                SkillEditorHelper.currConfigData.AnimTime = m_AnimTime;
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
                SkillEditorHelper.currConfigData.EXP = m_Exp;
                ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.currConfigData.Type = (SkillConfigData.SkillType)EditorGUILayout.EnumPopup("SkillType", SkillEditorHelper.currConfigData.Type);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.currConfigData.DeployerType = (SkillConfigData.SkillDeployerType)EditorGUILayout.EnumPopup("SkillDeployerType", SkillEditorHelper.currConfigData.DeployerType);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.currConfigData.TriggerType = (SkillConfigData.SkillTriggerType)EditorGUILayout.EnumPopup("SkillTriggerType", SkillEditorHelper.currConfigData.TriggerType);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.currConfigData.IsInEffectPlaySound = EditorGUILayout.Toggle("EffectPlaySound", SkillEditorHelper.currConfigData.IsInEffectPlaySound);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.currConfigData.CanChangeDir = EditorGUILayout.Toggle("CanChangeDir", SkillEditorHelper.currConfigData.CanChangeDir);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            SkillEditorHelper.currConfigData.CanMove = EditorGUILayout.Toggle("CanMove", SkillEditorHelper.currConfigData.CanMove);
        });

        GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("SkillKey");
            SkillEditorHelper.currConfigData.Key.AddTrigger = EditorGUILayout.Toggle("AddTrigger", SkillEditorHelper.currConfigData.Key.AddTrigger);

            int removeKeyIndex = -1;

            for (int i = 0; i < SkillEditorHelper.currConfigData.Key.Keys.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                SkillEditorHelper.currConfigData.Key.Keys[i] = (GameFrameWork.Input.KeyType)EditorGUILayout.EnumPopup(SkillEditorHelper.currConfigData.Key.Keys[i]);
                if (GUILayout.Button("x", GUILayout.Width(20)))
                {
                    removeKeyIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            if(removeKeyIndex >= 0)
            {
                m_ListKey.RemoveAt(removeKeyIndex);
                SkillEditorHelper.currConfigData.Key.Keys = m_ListKey.ToArray();
                removeKeyIndex = -1;
            }

            if (GUILayout.Button("增加按键"))
            {
                m_ListKey.Add(GameFrameWork.Input.KeyType.A);
                SkillEditorHelper.currConfigData.Key.Keys = m_ListKey.ToArray();
            }

            if (GUILayout.Button("默认重力"))
            {
                for (int i = 0; i < SkillEditorHelper.skillDatas.Count; i++)
                {
                    for (int j = 0; j < SkillEditorHelper.skillDatas[i].SkillEffects.Length; j++)
                    {
                        SkillEditorHelper.skillDatas[i].SkillEffects[j].Gravity = 1f;
                    }
                };
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