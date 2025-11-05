using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillAnimEventGUI : SkillEventGUI
    {
        private AnimEventInfo m_CurrAnimEventInfo;
        
        public SkillAnimEventGUI(EditorWindow window) : base(window)
        {
            m_CurrAnimEventInfo = new AnimEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            currEvent.animEventInfo ??= new();
            m_CurrAnimEventInfo ??= new();
            m_CurrAnimEventInfo.animName = currEvent.animEventInfo.animName;
            m_CurrAnimEventInfo.animSpeed = currEvent.animEventInfo.animSpeed;
            m_CurrAnimEventInfo.animPlayTimes = currEvent.animEventInfo.animPlayTimes;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawField(() => { return m_CurrAnimEventInfo.animName != currEvent.animEventInfo.animName; },
                () => { m_CurrAnimEventInfo.animName = EditorGUILayout.TextField("动画名称", m_CurrAnimEventInfo.animName); },
                () => { currEvent.animEventInfo.animName = m_CurrAnimEventInfo.animName; });

            DrawField(() => { return !Mathf.Approximately(m_CurrAnimEventInfo.animSpeed, currEvent.animEventInfo.animSpeed); },
                () => { m_CurrAnimEventInfo.animSpeed = EditorGUILayout.FloatField("动画速度", m_CurrAnimEventInfo.animSpeed); },
                () => { currEvent.animEventInfo.animSpeed = m_CurrAnimEventInfo.animSpeed; });

            DrawField(() => { return !Mathf.Approximately(m_CurrAnimEventInfo.animPlayTimes, currEvent.animEventInfo.animPlayTimes); },
                () => { m_CurrAnimEventInfo.animPlayTimes = EditorGUILayout.FloatField("播放次数", m_CurrAnimEventInfo.animPlayTimes); },
                () => { currEvent.animEventInfo.animPlayTimes = m_CurrAnimEventInfo.animPlayTimes; });

            EditorGUILayout.EndVertical();
        }

        protected override void OnResetEvent()
        {
            m_CurrAnimEventInfo = null;
        }
    }
}
