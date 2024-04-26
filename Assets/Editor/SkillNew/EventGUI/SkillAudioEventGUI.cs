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

            DrawField(() => { return m_CurrAudioClipName != m_CurrEvent.audioClipName; },
                () => { m_CurrAudioClipName = EditorGUILayout.TextField("音频名称", m_CurrAudioClipName); },
                () => { m_CurrEvent.audioClipName = m_CurrAudioClipName; }, 20);

            DrawField(() => { return m_CurrAudioPlaySpeed != m_CurrEvent.audioPlaySpeed; },
                () => { m_CurrAudioPlaySpeed = EditorGUILayout.Slider("播放速度", m_CurrAudioPlaySpeed, 0f,1f); },
                () => { m_CurrEvent.audioPlaySpeed = m_CurrAudioPlaySpeed; }, 20);

            DrawField(() => { return m_CurrAudioPlayVolume != m_CurrEvent.audioPlayVolume; },
                () => { m_CurrAudioPlayVolume = EditorGUILayout.Slider("播放音量", m_CurrAudioPlayVolume, -3f, 3f); },
                () => { m_CurrEvent.audioPlayVolume = m_CurrAudioPlayVolume; }, 20);

            DrawField(() => { return m_CurrAudioPlayLoop != m_CurrEvent.audioPlayLoop; },
                () => { m_CurrAudioPlayLoop = EditorGUILayout.Toggle("循环播放", m_CurrAudioPlayLoop); },
                () => { m_CurrEvent.audioPlayLoop = m_CurrAudioPlayLoop; }, 20);

            EditorGUILayout.EndVertical();
        }

        public override void ResetEvent()
        {
            base.ResetEvent();
            m_CurrAudioClipName = string.Empty;
            m_CurrAudioPlaySpeed = 0f;
            m_CurrAudioPlayLoop = false;
            m_CurrAudioPlayVolume = 0f;
            m_CurrEvent.audioClipName = string.Empty;
            m_CurrEvent.audioPlaySpeed = 0f;
            m_CurrEvent.audioPlayVolume = 0f;
            m_CurrEvent.audioPlayLoop = false;
        }

        private string m_CurrAudioClipName;
        private float m_CurrAudioPlaySpeed;
        private bool m_CurrAudioPlayLoop;
        private float m_CurrAudioPlayVolume;
    }
}
