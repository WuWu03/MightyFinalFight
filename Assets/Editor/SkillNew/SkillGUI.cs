using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public class SkillGUI
    {
        public SkillGUI(EditorWindow window)
        {
            m_EditorWindow = window;
        }

        public void UpdateData()
        {
            OnUpdateData();
        }

        public void Draw()
        {
            OnGUI();
        }

        protected virtual void OnUpdateData() { }
        protected virtual void OnGUI() { }

        protected EditorWindow m_EditorWindow = null;
    }
}