using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public class SkillAnimEventGUI : SkillEventGUI
    {
        public SkillAnimEventGUI(EditorWindow window) : base(window)
        {

        }

        protected override void OnUpdateSkillEvent()
        {
            m_CurrAnimName = m_CurrEvent.animName;
            m_CurrAnimSpeed = m_CurrEvent.animSpeed;
            m_CurrAnimPlayTimes = m_CurrEvent.animPlayTimes;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (m_CurrEvent == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            bool isModify = m_CurrAnimName != m_CurrEvent.animName;
            m_CurrAnimName = EditorGUILayout.TextField("动画名称", m_CurrAnimName, SkillEditorHelper.GetTextFieldStyle(isModify));

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrEvent.animName = m_CurrAnimName;
                this.m_EditorWindow.ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            isModify = m_CurrAnimSpeed != m_CurrEvent.animSpeed;
            m_CurrAnimSpeed = EditorGUILayout.FloatField("动画速度", m_CurrAnimSpeed, SkillEditorHelper.GetTextFieldStyle(isModify));

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrEvent.animSpeed = m_CurrAnimSpeed;
                this.m_EditorWindow.ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            isModify = m_CurrAnimPlayTimes != m_CurrEvent.animPlayTimes;
            m_CurrAnimPlayTimes = EditorGUILayout.FloatField("播放次数", m_CurrAnimPlayTimes, SkillEditorHelper.GetTextFieldStyle(isModify));

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrEvent.animPlayTimes = m_CurrAnimPlayTimes;
                this.m_EditorWindow.ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private string m_CurrAnimName;
        private float m_CurrAnimSpeed;
        private float m_CurrAnimPlayTimes;
    }
}
