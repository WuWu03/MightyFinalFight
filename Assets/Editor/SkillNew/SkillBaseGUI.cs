using System;
using UnityEditor;

namespace SkillNew
{
    public class SkillBaseGUI
    {
        public SkillBaseGUI(EditorWindow window)
        {
            m_EditorWindow = window;
        }

        public void UpdateData()
        {
            OnUpdateData();
        }

        public virtual void Draw()
        {
            OnGUI();
        }

        protected void DrawField(Func<bool> modify, Action draw, Action change, int changeBtnHeight = 20, bool showMsg = true)
        {
            (m_EditorWindow as SkillEditorWindow).DrawField(modify, draw, change, changeBtnHeight, showMsg);
        }

        protected virtual void OnUpdateData() { }
        protected virtual void OnGUI() { }

        protected EditorWindow m_EditorWindow = null;
    }
}