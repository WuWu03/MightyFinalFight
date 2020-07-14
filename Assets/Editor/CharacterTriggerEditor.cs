using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DragonBones;
using System;
using FrameWork;

public class CharacterTriggerEditor : EditorWindow
{
    private void OnEnable()
    {
        m_ListGO = new List<GameObject>();
        
    }

    private void OnDisable()
    {
        for(int i = m_ListGO.Count - 1; i > -1; i--)
        {
            GameObject.DestroyImmediate(m_ListGO[i]);
        }

        m_ListGO.Clear();
    }
    private void OnGUI()
    {
        SetSelection();
        OnItemSelect();
        SetTriggerData();
        this.Repaint();
    }

    private void SetSelection()
    {
        m_CurrSelectObj = EditorGUILayout.ObjectField("拖入龙骨角色", m_CurrSelectObj, typeof(UnityArmatureComponent), false);

        if (m_CurrSelectObj == null)
        {
            return;
        }

        if(m_CurrGo == null || m_CurrGo.name != m_CurrSelectObj.name)
        {
            m_CurrGo = GetSelectObj(m_CurrSelectObj.name);
            m_CurrDB = m_CurrGo.GetOrAddComponent<UnityArmatureComponent>();

            if (m_CurrDB == null)
            {
                return;
            }

            m_CurrCollider = m_CurrGo.GetOrAddComponent<BoxCollider2D>();
            m_DBTriggers = new TriggerData[m_CurrDB.animation.animationNames.Count];
            TriggerData[] currDB = m_CurrGo.GetOrAddComponent<DBTrigger>().TriggerDatas;

            if(currDB == null)
            {
                m_CurrGo.GetOrAddComponent<DBTrigger>().TriggerDatas = new TriggerData[0];
                currDB = m_CurrGo.GetOrAddComponent<DBTrigger>().TriggerDatas;
            }

            int dbIndex = 0;
            for(int i = 0; i < currDB.Length; i++)
            {
                bool hasFind = false;
                for(int j = 0; j < m_CurrDB.animation.animationNames.Count; j++)
                {
                    if(m_CurrDB.animation.animationNames[j].Equals(currDB[i].AnimName))
                    {
                        hasFind = true;
                        break;
                    }
                }
                if (hasFind)
                {
                    m_DBTriggers[dbIndex] = currDB[i];
                    dbIndex++;
                }
            }

            for (int i = 0; i < m_CurrDB.animation.animationNames.Count; i++)
            {
                if (m_DBTriggers[i] == null || !m_DBTriggers[i].AnimName.Equals(m_CurrDB.animation.animationNames[i]))
                {
                    m_DBTriggers[i] = new TriggerData()
                    {
                        AnimName = m_CurrDB.animation.animationNames[i],
                        Offest = Vector2.zero,
                        Size = Vector2.one,
                    };
                }
            }

            if (m_DBTriggers.Length - currDB.Length > 0)
            {
                for (int i = currDB.Length; i < m_DBTriggers.Length; i++)
                {
                    m_DBTriggers[i] = new TriggerData()
                    {
                        AnimName = m_CurrDB.animation.animationNames[i],
                        Offest = Vector2.zero,
                        Size = Vector2.one,
                    };
                }
            }

            Selection.activeGameObject = m_CurrGo;
        }   
    }

    private void SetTriggerData()
    {
        if (m_CurrDB == null)
        {
            return;
        }

        if (m_CurrTriggerData == null)
        {
            return;
        }

        m_CurrAnimIndex = EditorGUILayout.Popup("选择动画",m_CurrAnimIndex, m_CurrDB.animation.animationNames.ToArray());
        m_CurrCollider.offset = EditorGUILayout.Vector2Field("当前触发器偏移:", m_CurrCollider.offset);
        m_CurrCollider.size = EditorGUILayout.Vector2Field("当前触发器尺寸:", m_CurrCollider.size);

        if (GUILayout.Button("保存设置"))
        {
            m_CurrTriggerData.Offest = m_CurrCollider.offset;
            m_CurrTriggerData.Size = m_CurrCollider.size;

            m_CurrDB.GetComponent<DBTrigger>().TriggerDatas = m_DBTriggers;
            Component.DestroyImmediate(m_CurrCollider);
            m_CurrAnimName = string.Empty;
            bool isSuccess = false;
            PrefabUtility.SaveAsPrefabAsset(m_CurrDB.gameObject, AssetDatabase.GetAssetPath(m_CurrSelectObj), out isSuccess);

            if (isSuccess)
            {
                
            }

            m_CurrCollider = m_CurrGo.GetOrAddComponent<BoxCollider2D>();
        }

        if (GUILayout.Button("恢复初始"))
        {
            m_CurrCollider.offset = m_CurrTriggerData.Offest;
            m_CurrCollider.size = m_CurrTriggerData.Size;
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

    private void OnItemSelect()
    {
        if (m_CurrDB == null || m_CurrCollider == null || m_CurrAnimName == m_CurrDB.animation.animationNames[m_CurrAnimIndex]) return;

        m_CurrAnimName = m_CurrDB.animation.animationNames[m_CurrAnimIndex];
        m_CurrDB.animation.Play(m_CurrAnimName, 0);
        m_CurrDB.animation.timeScale = 0.1f;
        m_CurrTriggerData = GetDBTriggerByName(m_CurrAnimName);

        if (m_CurrTriggerData != null)
        {
            m_CurrCollider.offset = m_CurrTriggerData.Offest;
            m_CurrCollider.size = m_CurrTriggerData.Size;
        }
    }

    private GameObject GetSelectObj(string name)
    {
        for(int i = 0; i < m_ListGO.Count; i++)
        {
            if (m_ListGO[i].name.Equals(name))
            {
                return m_ListGO[i];
            }
        }

        GameObject ret = GameObject.Instantiate((m_CurrSelectObj as UnityArmatureComponent).gameObject) as GameObject;
        ret.name = m_CurrSelectObj.name;
        m_ListGO.Add(ret);
        return ret;
    }


    private int m_CurrAnimIndex = 0;
    private List<GameObject> m_ListGO = null;
    private GameObject m_CurrGo = null;
    private UnityEngine.Object m_CurrSelectObj = null;
    private UnityArmatureComponent m_CurrDB = null;
    private BoxCollider2D m_CurrCollider = null;
    private TriggerData m_CurrTriggerData = null;
    private string m_CurrAnimName = string.Empty;
    private Vector2 scroll = Vector2.zero;
    private TriggerData[] m_DBTriggers = null;
}
