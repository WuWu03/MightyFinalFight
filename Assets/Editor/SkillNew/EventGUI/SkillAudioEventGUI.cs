using UnityEditor;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillAudioEventGUI : SkillEventGUI
    {
        public SkillAudioEventGUI(EditorWindow window) : base(window)
        {
            m_CurrAudioEventInfo = new AudioEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            if (m_CurrEvent.audioEventInfo == null)
            {
                m_CurrEvent.audioEventInfo = new AudioEventInfo();
            }

            m_CurrAudioEventInfo.audioClipName = m_CurrEvent.audioEventInfo.audioClipName;
            m_CurrAudioEventInfo.audioPlaySpeed = m_CurrEvent.audioEventInfo.audioPlaySpeed;
            m_CurrAudioEventInfo.audioPlayLoop = m_CurrEvent.audioEventInfo.audioPlayLoop;
            m_CurrAudioEventInfo.audioPlayVolume = m_CurrEvent.audioEventInfo.audioPlayVolume;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawField(() => { return m_CurrAudioEventInfo.audioClipName != m_CurrEvent.audioEventInfo.audioClipName; },
                () => { m_CurrAudioEventInfo.audioClipName = EditorGUILayout.TextField("音频名称", m_CurrAudioEventInfo.audioClipName); },
                () => { m_CurrEvent.audioEventInfo.audioClipName = m_CurrAudioEventInfo.audioClipName; });

            DrawField(() => { return m_CurrAudioEventInfo.audioPlaySpeed != m_CurrEvent.audioEventInfo.audioPlaySpeed; },
                () => { m_CurrAudioEventInfo.audioPlaySpeed = EditorGUILayout.Slider("播放速度", m_CurrAudioEventInfo.audioPlaySpeed, 0f,1f); },
                () => { m_CurrEvent.audioEventInfo.audioPlaySpeed = m_CurrAudioEventInfo.audioPlaySpeed; });

            DrawField(() => { return m_CurrAudioEventInfo.audioPlayVolume != m_CurrEvent.audioEventInfo.audioPlayVolume; },
                () => { m_CurrAudioEventInfo.audioPlayVolume = EditorGUILayout.Slider("播放音量", m_CurrAudioEventInfo.audioPlayVolume, -3f, 3f); },
                () => { m_CurrEvent.audioEventInfo.audioPlayVolume = m_CurrAudioEventInfo.audioPlayVolume; });

            DrawField(() => { return m_CurrAudioEventInfo.audioPlayLoop != m_CurrEvent.audioEventInfo.audioPlayLoop; },
                () => { m_CurrAudioEventInfo.audioPlayLoop = EditorGUILayout.Toggle("循环播放", m_CurrAudioEventInfo.audioPlayLoop); },
                () => { m_CurrEvent.audioEventInfo.audioPlayLoop = m_CurrAudioEventInfo.audioPlayLoop; });

            EditorGUILayout.EndVertical();
        }

        protected override void OnResetEvent()
        {
            m_CurrEvent.audioEventInfo = null;
        }

        private AudioEventInfo m_CurrAudioEventInfo = null;
    }
}
