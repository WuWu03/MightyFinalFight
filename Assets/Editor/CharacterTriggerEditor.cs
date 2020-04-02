using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DragonBones;
using System;
using FrameWork;

public class CharacterTriggerEditor : EditorWindow
{
    private void OnInspectorUpdate()
    {
        if(m_CurrCollider != null && m_CurrCollider != null)
        {
            //SetSelection();
            //SetTriggerData();
        }
    }

    private void OnEnable()
    {
        
    }
    private void OnGUI()
    {
        SetSelection();
        SetTriggerData();
        this.Repaint();
    }

    private void SetSelection()
    {
        if (Selection.objects.Length < 1) return;
        GameObject go = Selection.objects[0] as GameObject;
        if (go == null) return;

        m_CurrDB = go.GetComponent<UnityArmatureComponent>();

        if (m_CurrDB == null) return;

        m_CurrCollider = go.GetOrAddComponent<BoxCollider2D>();
        m_DBTriggers = go.GetComponent<DBTrigger>().TriggerDatas;

        if (m_DBTriggers == null || m_DBTriggers.Length < 1)
        {
            m_DBTriggers = new TriggerData[m_CurrDB.animation.animationNames.Count];

            for (int i = 0; i < m_CurrDB.animation.animationNames.Count; i++)
            {
                m_DBTriggers[i] = new TriggerData()
                {
                    AnimName = m_CurrDB.animation.animationNames[i],
                    Offest = Vector2.zero,
                    Size = Vector2.zero,
                };
            }
        }  
    }

    private void SetTriggerData()
    {
        if (m_CurrDB == null) return;
        m_CurrTriggerData = GetDBTriggerByName(m_CurrDB.animationName);
        if (m_CurrTriggerData == null) return;

        EditorGUILayout.LabelField("当前动画:", m_CurrDB.animationName);
        EditorGUILayout.Vector2Field("当前触发器偏移:", m_CurrCollider.offset);
        EditorGUILayout.Vector2Field("当前触发器尺寸:", m_CurrCollider.size);
        EditorGUILayout.Vector2Field("记录触发器便宜:", m_CurrTriggerData.Offest);
        EditorGUILayout.Vector2Field("记录触发器尺寸:", m_CurrTriggerData.Size);

        if (GUILayout.Button("保存设置"))
        {
            m_CurrTriggerData.Offest = m_CurrCollider.offset;
            m_CurrTriggerData.Size = m_CurrCollider.size;

            m_CurrDB.GetComponent<DBTrigger>().TriggerDatas = m_DBTriggers;
            Component.Destroy(m_CurrCollider);
            Debug.Log(AssetDatabase.GetAssetPath(m_CurrDB.gameObject));
            PrefabUtility.ApplyObjectOverride(m_CurrDB.gameObject, AssetDatabase.GetAssetPath(m_CurrDB.gameObject), InteractionMode.AutomatedAction);
        }
    }

    private TriggerData GetDBTriggerByName(string name)
    {
        for (int i = 0; i < m_DBTriggers.Length; i++)
        {
            if (m_DBTriggers[i].AnimName.Equals(name))
            {
                return m_DBTriggers[i];
            }
        }

        return null;
    }
    private void OnItemSelect(object value)
    {
        m_CurrAnimName = value.ToString();

    }

    private UnityArmatureComponent m_CurrDB = null;
    private BoxCollider2D m_CurrCollider = null;
    private TriggerData m_CurrTriggerData = null;
    private string m_CurrAnimName = null;
    private Vector2 scroll = Vector2.zero;
    private TriggerData[] m_DBTriggers = null;
}
