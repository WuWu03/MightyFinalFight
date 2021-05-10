using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameFrameWork.Serialize
{
    [Serializable]
    public class BehaviourTreeWindowNode
    {
        public Rect Rect
        {
            get
            {
                return m_RectWindow;
            }
        }


        private BehaviourTreeWindowConfig m_BehaviourTreeWindowConfig = null;
        //

        public BehaviourTreeWindowNode(int windowID, string name, float x, float y, float width, float height)
        {
            m_WindownID = windowID;
            m_Name = name;
            m_RectWindow = new Rect(x, y, width, height);
            m_BehaviourTreeWindowConfig = AssetDatabase.LoadAssetAtPath<BehaviourTreeWindowConfig>(PathUtil.BehaviourTreeWindowDataPath);
        }

        public void OnGUI()
        {
            m_RectWindow = GUI.Window(m_WindownID, m_RectWindow, DrawNodeWindow, m_Name);
        }

        void DrawNodeWindow(int id)
        {
            GUI.DragWindow();
        }

        private ReorderableList m_LeftList = null;
        private string m_Name = string.Empty;
        private Rect m_RectWindow;
        private int m_WindownID = 0;
    }
}