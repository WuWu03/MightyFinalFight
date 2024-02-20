using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DragonBones;
using System;
using GameFrameWork;
using Unity.VisualScripting;

public class CharacterTriggerEditorWindow : EditorWindow
{
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        if (m_CurrGo != null)
        {
            GameObject.DestroyImmediate(m_CurrGo);
            m_CurrGo = null;
        }
    }

    private void OnGUI()
    {
        if(m_CurrGo != null)
        {
            Selection.activeObject = m_CurrGo;

        }

        SetSelection();
        DrawScene();
        SetTriggerData();

        this.Repaint();
    }

    private void DrawScene()
    {

    }

    private void SetSelection()
    {
        m_CurrSelectObj = EditorGUILayout.ObjectField("拖入龙骨角色", m_CurrSelectObj, typeof(UnityArmatureComponent), false);
      
        if (m_CurrSelectObj == null)
        {
            return;
        }

        if(m_CurrGo == null || !m_CurrGo.name.Contains(m_CurrSelectObj.name))
        {
            if(m_CurrGo != null)
            {
                GameObject.DestroyImmediate(m_CurrGo);
                m_CurrGo = null;
            }

            m_CurrGo = GameObject.Instantiate((m_CurrSelectObj as UnityArmatureComponent).gameObject) as GameObject;
            m_CurrDB = m_CurrGo.GetOrAddComponent<UnityArmatureComponent>();

            if (m_CurrDB == null)
            {
                return;
            }

            m_CurrCollider = m_CurrGo.GetOrAddComponent<BoxCollider2D>();
            m_TriggerDatas = new TriggerData[m_CurrDB.animation.animationNames.Count];
            TriggerData[] currTriggerDatas = m_CurrGo.GetOrAddComponent<HitTrigger>().TriggerDatas;

            if(currTriggerDatas == null)
            {
                m_CurrGo.GetOrAddComponent<HitTrigger>().TriggerDatas = new TriggerData[0];
                currTriggerDatas = m_CurrGo.GetOrAddComponent<HitTrigger>().TriggerDatas;
            }

            int dbIndex = 0;

            for(int i = 0; i < currTriggerDatas.Length; i++)
            {
                bool hasFind = false;
                for(int j = 0; j < m_CurrDB.animation.animationNames.Count; j++)
                {
                    if(m_CurrDB.animation.animationNames[j].Equals(currTriggerDatas[i].animName))
                    {
                        hasFind = true;
                        break;
                    }
                }

                if (hasFind)
                {
                    m_TriggerDatas[dbIndex] = currTriggerDatas[i];
                    dbIndex++;
                }
            }

            for (int i = 0; i < m_CurrDB.animation.animationNames.Count; i++)
            {
                string animationName = m_CurrDB.animation.animationNames[i];
                int animationFrameCount = GetFrameCount(animationName);

                if (m_TriggerDatas[i] == null || !m_TriggerDatas[i].animName.Equals(m_CurrDB.animation.animationNames[i]))
                {
                    m_TriggerDatas[i] = new TriggerData()
                    {
                        animName = animationName,
                        offestList = new Vector2[animationFrameCount],
                        sizeList = new Vector2[animationFrameCount],
                    };
                }
                else if (m_TriggerDatas[i].sizeList.Length != animationFrameCount)
                {
                    m_TriggerDatas[i].sizeList = new Vector2[animationFrameCount];
                    m_TriggerDatas[i].offestList = new Vector2[animationFrameCount];
                }
            }

            if (m_TriggerDatas.Length - currTriggerDatas.Length > 0)
            {
                for (int i = currTriggerDatas.Length; i < m_TriggerDatas.Length; i++)
                {
                    string animationName = m_CurrDB.animation.animationNames[i];
                    int animationFrameCount = GetFrameCount(animationName);

                    m_TriggerDatas[i] = new TriggerData()
                    {
                        animName = animationName,
                        offestList = new Vector2[animationFrameCount],
                        sizeList = new Vector2[animationFrameCount],
                    };
                }
            }

            Selection.activeGameObject = m_CurrGo;
            m_CurrAnimIndex = 0;
            m_CurrAnimFrame = 1;
            m_CurrAnimName = m_CurrDB.animation.animationNames[0];

            OnItemSelect();
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

        int selectAnimIndex = EditorGUILayout.Popup("选择动画", m_CurrAnimIndex, m_CurrDB.animation.animationNames.ToArray());
        string animName = m_CurrDB.animation.animationNames[m_CurrAnimIndex];
        int animFrame = EditorGUILayout.IntSlider("当前帧:", m_CurrAnimFrame, 1, GetFrameCount(animName));

        m_CurrCollider.offset = EditorGUILayout.Vector2Field("当前触发器偏移:", m_CurrCollider.offset);
        m_CurrCollider.size = EditorGUILayout.Vector2Field("当前触发器尺寸:", m_CurrCollider.size);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("复制"))
        {
            m_CopySize = m_CurrCollider.size;
            m_CopyOffest = m_CurrCollider.offset;
        }

        if(m_CopySize != Vector2.zero || m_CopyOffest != Vector2.zero)
        {
            if (GUILayout.Button("粘贴"))
            {
                m_CurrCollider.size = m_CopySize;
                m_CurrCollider.offset = m_CopyOffest;
            }

            if (GUILayout.Button("粘贴全部"))
            {
                for (int i = 0; i < m_CurrTriggerData.offestList.Length; i++)
                {
                    m_CurrTriggerData.offestList[i] = m_CopyOffest;
                    m_CurrTriggerData.sizeList[i] = m_CopySize;
                }

                m_CurrCollider.size = m_CopySize;
                m_CurrCollider.offset = m_CopyOffest;
            }
        }
  
        EditorGUILayout.EndHorizontal();

        bool canSelect = false;

        if (m_CurrAnimIndex != selectAnimIndex )
        {
            m_CurrAnimFrame = 1;
            m_CurrAnimIndex = selectAnimIndex;
            m_CurrAnimName = m_CurrDB.animation.animationNames[m_CurrAnimIndex];
            canSelect = true;
        }
        else if (m_CurrAnimFrame != animFrame)
        {
            m_CurrAnimFrame = animFrame;
            canSelect = true;
        }

        if (canSelect)
        {
            OnItemSelect();
        }

        if (GUILayout.Button("保存设置"))
        {
            m_CurrTriggerData.offestList[m_CurrAnimFrame - 1] = m_CurrCollider.offset;
            m_CurrTriggerData.sizeList[m_CurrAnimFrame - 1] = m_CurrCollider.size;

            m_CurrDB.GetComponent<HitTrigger>().TriggerDatas = m_TriggerDatas;
            Component.DestroyImmediate(m_CurrCollider);

            PrefabUtility.SaveAsPrefabAsset(m_CurrDB.gameObject, AssetDatabase.GetAssetPath(m_CurrSelectObj), out bool isSuccess);

            if (isSuccess)
            {

            }

            m_CurrCollider = m_CurrGo.GetOrAddComponent<BoxCollider2D>();
            m_CurrCollider.size = m_CurrTriggerData.sizeList[m_CurrAnimFrame - 1];
            m_CurrCollider.offset = m_CurrTriggerData.offestList[m_CurrAnimFrame - 1];
        }

        if (GUILayout.Button("恢复初始"))
        {
            m_CurrCollider.offset = m_CurrTriggerData.offestList[m_CurrAnimFrame - 1];
            m_CurrCollider.size = m_CurrTriggerData.sizeList[m_CurrAnimFrame - 1];
        }
    }

    private TriggerData GetTriggerData(string name)
    {
        for (int i = 0; i < m_TriggerDatas.Length; i++)
        {
            if (m_TriggerDatas[i].animName.Equals(name))
            {
                return m_TriggerDatas[i];
            }
        }

        return null;
    }

    private void OnItemSelect()
    {
        m_CurrTriggerData = GetTriggerData(m_CurrAnimName);

        if (m_CurrTriggerData != null)
        {
            if (m_CurrTriggerData.sizeList[m_CurrAnimFrame - 1] != Vector2.zero)
            {
                m_CurrCollider.size = m_CurrTriggerData.sizeList[m_CurrAnimFrame - 1];
            }
            else
            {
                m_CurrCollider.size = Vector2.one;
            }

            m_CurrCollider.offset = m_CurrTriggerData.offestList[m_CurrAnimFrame - 1];
        }

        m_CurrDB.animation.GotoAndStopByFrame(m_CurrAnimName, (uint)(m_CurrAnimFrame - 1));
    }

    private int GetFrameCount(string animName)
    {
        if(m_CurrDB == null)
        {
            return 0;
        }

        return m_CurrDB.animation.animations[animName].frameCount > 1 ? (int)m_CurrDB.animation.animations[animName].frameCount + 1 : 1;
    }

    private int m_CurrAnimIndex = 0;
    private int m_CurrAnimFrame = 1;
    private int m_PrevAnimFrame = -1;

    private GameObject m_CurrGo = null;
    private UnityEngine.Object m_CurrSelectObj = null;
    private UnityArmatureComponent m_CurrDB = null;
    private BoxCollider2D m_CurrCollider = null;
    private TriggerData m_CurrTriggerData = null;
    private string m_CurrAnimName = string.Empty;
    private Vector2 scroll = Vector2.zero;
    private Vector2 m_CopyOffest = Vector2.zero;
    private Vector2 m_CopySize = Vector2.zero;
    private TriggerData[] m_TriggerDatas = null;
}
