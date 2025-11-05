using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillAudioEventGUI : SkillEventGUI
    {
        private AudioEventInfo m_CurrAudioEventInfo;
        
        public SkillAudioEventGUI(EditorWindow window) : base(window)
        {
            m_CurrAudioEventInfo = new AudioEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            currEvent.audioEventInfo ??= new();
            m_CurrAudioEventInfo ??= new();
            m_CurrAudioEventInfo.audioClipName = currEvent.audioEventInfo.audioClipName;
            m_CurrAudioEventInfo.audioPlaySpeed = currEvent.audioEventInfo.audioPlaySpeed;
            m_CurrAudioEventInfo.audioPlayLoop = currEvent.audioEventInfo.audioPlayLoop;
            m_CurrAudioEventInfo.audioPlayVolume = currEvent.audioEventInfo.audioPlayVolume;
        }

        protected override void OnGUI()
        {
            DrawField(() => { return m_CurrAudioEventInfo.audioClipName != currEvent.audioEventInfo.audioClipName; },
                () => { m_CurrAudioEventInfo.audioClipName = EditorGUILayout.TextField("音频名称", m_CurrAudioEventInfo.audioClipName); },
                () => { currEvent.audioEventInfo.audioClipName = m_CurrAudioEventInfo.audioClipName; });

            DrawField(() => { return !Mathf.Approximately(m_CurrAudioEventInfo.audioPlaySpeed, currEvent.audioEventInfo.audioPlaySpeed); },
                () => { m_CurrAudioEventInfo.audioPlaySpeed = EditorGUILayout.Slider("播放速度", m_CurrAudioEventInfo.audioPlaySpeed, 0f,1f); },
                () => { currEvent.audioEventInfo.audioPlaySpeed = m_CurrAudioEventInfo.audioPlaySpeed; });

            DrawField(() => { return !Mathf.Approximately(m_CurrAudioEventInfo.audioPlayVolume, currEvent.audioEventInfo.audioPlayVolume); },
                () => { m_CurrAudioEventInfo.audioPlayVolume = EditorGUILayout.Slider("播放音量", m_CurrAudioEventInfo.audioPlayVolume, -3f, 3f); },
                () => { currEvent.audioEventInfo.audioPlayVolume = m_CurrAudioEventInfo.audioPlayVolume; });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawField(() => { return m_CurrAudioEventInfo.audioPlayLoop != currEvent.audioEventInfo.audioPlayLoop; },
                () => { m_CurrAudioEventInfo.audioPlayLoop = EditorGUILayout.Toggle("循环播放", m_CurrAudioEventInfo.audioPlayLoop); },
                () => { currEvent.audioEventInfo.audioPlayLoop = m_CurrAudioEventInfo.audioPlayLoop; });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        protected override void OnResetEvent()
        {
            m_CurrAudioEventInfo = null;
        }
    }
}
