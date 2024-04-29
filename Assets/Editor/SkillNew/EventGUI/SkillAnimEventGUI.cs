using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData;

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

            EditorGUILayout.BeginVertical();

            DrawField(() => { return m_CurrAnimName != m_CurrEvent.animName; },
                () => { m_CurrAnimName = EditorGUILayout.TextField("动画名称", m_CurrAnimName); },
                () => { m_CurrEvent.animName = m_CurrAnimName; }, 20);

            DrawField(() => { return m_CurrAnimSpeed != m_CurrEvent.animSpeed; },
                () => { m_CurrAnimSpeed = EditorGUILayout.FloatField("动画速度", m_CurrAnimSpeed); },
                () => { m_CurrEvent.animSpeed = m_CurrAnimSpeed; }, 20);

            DrawField(() => { return m_CurrAnimPlayTimes != m_CurrEvent.animPlayTimes; },
                () => { m_CurrAnimPlayTimes = EditorGUILayout.FloatField("播放次数", m_CurrAnimPlayTimes); },
                () => { m_CurrEvent.animPlayTimes = m_CurrAnimPlayTimes; }, 20);

            EditorGUILayout.EndVertical();
        }

        protected override void OnResetEvent()
        {
            base.ResetEvent();

            m_CurrEvent.animName = string.Empty;
            m_CurrEvent.animSpeed = 0f;
            m_CurrEvent.animPlayTimes = 0f;

            m_CurrAnimName = string.Empty;
            m_CurrAnimSpeed = 0f;
            m_CurrAnimPlayTimes = 0f;

        }

        private string m_CurrAnimName;
        private float m_CurrAnimSpeed;
        private float m_CurrAnimPlayTimes;
    }
}
