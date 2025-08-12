using UnityEditor;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillAnimEventGUI : SkillEventGUI
    {
        public SkillAnimEventGUI(EditorWindow window) : base(window)
        {
            m_CurrAnimEventInfo = new AnimEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            if (m_CurrEvent.animEventInfo == null)
            {
                m_CurrEvent.animEventInfo = new AnimEventInfo();
            }

            m_CurrAnimEventInfo.animName = m_CurrEvent.animEventInfo.animName;
            m_CurrAnimEventInfo.animSpeed = m_CurrEvent.animEventInfo.animSpeed;
            m_CurrAnimEventInfo.animPlayTimes = m_CurrEvent.animEventInfo.animPlayTimes;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            DrawField(() => { return m_CurrAnimEventInfo.animName != m_CurrEvent.animEventInfo.animName; },
                () => { m_CurrAnimEventInfo.animName = EditorGUILayout.TextField("动画名称", m_CurrAnimEventInfo.animName); },
                () => { m_CurrEvent.animEventInfo.animName = m_CurrAnimEventInfo.animName; });

            DrawField(() => { return m_CurrAnimEventInfo.animSpeed != m_CurrEvent.animEventInfo.animSpeed; },
                () => { m_CurrAnimEventInfo.animSpeed = EditorGUILayout.FloatField("动画速度", m_CurrAnimEventInfo.animSpeed); },
                () => { m_CurrEvent.animEventInfo.animSpeed = m_CurrAnimEventInfo.animSpeed; });

            DrawField(() => { return m_CurrAnimEventInfo.animPlayTimes != m_CurrEvent.animEventInfo.animPlayTimes; },
                () => { m_CurrAnimEventInfo.animPlayTimes = EditorGUILayout.FloatField("播放次数", m_CurrAnimEventInfo.animPlayTimes); },
                () => { m_CurrEvent.animEventInfo.animPlayTimes = m_CurrAnimEventInfo.animPlayTimes; });

            EditorGUILayout.EndVertical();
        }

        protected override void OnResetEvent()
        {
            m_CurrEvent.animEventInfo = null;
        }

        private AnimEventInfo m_CurrAnimEventInfo = null;
    }
}
