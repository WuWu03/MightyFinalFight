using UnityEditor;
using UnityEngine;

namespace SkillNew
{
    public class SkillAudioEventGUI : SkillEventGUI
    {
        public SkillAudioEventGUI(EditorWindow window) : base(window)
        {
        }

        protected override void OnUpdateSkillEvent()
        {
            m_CurrAudioClipName = m_CurrEvent.audioClipName;
            m_CurrAudioPlaySpeed = m_CurrEvent.audioPlaySpeed;
            m_CurrAudioPlayLoop = m_CurrEvent.audioPlayLoop;
            m_CurrAudioPlayVolume = m_CurrEvent.audioPlayVolume;
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
            bool isModify = m_CurrAudioClipName != m_CurrEvent.audioClipName;
            m_CurrAudioClipName = EditorGUILayout.TextField("音频名称", m_CurrAudioClipName, SkillEditorHelper.GetTextFieldStyle(isModify));

            if (GUILayout.Button("更改", GUILayout.Width(100)))
            {
                m_CurrEvent.audioClipName = m_CurrAudioClipName;
                this.m_EditorWindow.ShowNotification("更改成功");
            }
            EditorGUILayout.EndHorizontal();

            m_CurrAudioPlaySpeed = EditorGUILayout.Slider("播放速度", m_CurrAudioPlaySpeed, 0f, 1f);

            if (m_CurrAudioPlaySpeed != m_CurrEvent.audioPlaySpeed)
            {
                m_CurrEvent.audioPlaySpeed = m_CurrAudioPlaySpeed;
            }

            m_CurrAudioPlayVolume = EditorGUILayout.Slider("播放音量", m_CurrAudioPlayVolume, -3f, 3f);

            if (m_CurrAudioPlayVolume != m_CurrEvent.audioPlayVolume)
            {
                m_CurrEvent.audioPlayVolume = m_CurrAudioPlayVolume;
            }

            m_CurrAudioPlayLoop = EditorGUILayout.Toggle("循环播放", m_CurrAudioPlayLoop);

            if (m_CurrAudioPlayLoop != m_CurrEvent.audioPlayLoop)
            {
                m_CurrEvent.audioPlayLoop = m_CurrAudioPlayLoop;
            }

            EditorGUILayout.EndVertical();
        }

        private string m_CurrAudioClipName;
        private float m_CurrAudioPlaySpeed;
        private bool m_CurrAudioPlayLoop;
        private float m_CurrAudioPlayVolume;
    }
}
