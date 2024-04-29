using UnityEditor;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillPhysicsEventGUI : SkillEventGUI
    {
        public SkillPhysicsEventGUI(EditorWindow window) : base(window)
        {
            m_TargetPhysicsEventInfo = new PhysicsEventInfo();
            m_SelfPhysicsEventInfo = new PhysicsEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            PhysicsEventInfo currPhysicsEventInfo = null;
            PhysicsEventInfo physicsEventInfo = null;

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetPhysicsEvent)
            {
                if (m_CurrEvent.targetPhysicsEventInfo == null)
                {
                    m_CurrEvent.targetPhysicsEventInfo = new PhysicsEventInfo();
                }

                currPhysicsEventInfo = m_TargetPhysicsEventInfo;
                physicsEventInfo = m_CurrEvent.targetPhysicsEventInfo;
            }
            else if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfPhysicsEvent)
            {
                if (m_CurrEvent.selfPhysicsEventInfo == null)
                {
                    m_CurrEvent.selfPhysicsEventInfo = new PhysicsEventInfo();
                }

                currPhysicsEventInfo = m_SelfPhysicsEventInfo;
                physicsEventInfo = m_CurrEvent.selfPhysicsEventInfo;
            }

            currPhysicsEventInfo.force = physicsEventInfo.force;
            currPhysicsEventInfo.velocity = physicsEventInfo.velocity;
            currPhysicsEventInfo.drag = physicsEventInfo.drag;
            currPhysicsEventInfo.gravity = physicsEventInfo.gravity;
            currPhysicsEventInfo.distanceLimit = physicsEventInfo.distanceLimit;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            PhysicsEventInfo currPhysicsEventInfo = null;
            PhysicsEventInfo physicsEventInfo = null;

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetPhysicsEvent)
            {
                currPhysicsEventInfo = m_TargetPhysicsEventInfo;
                physicsEventInfo = m_CurrEvent.targetPhysicsEventInfo;
            }
            else if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfPhysicsEvent)
            {
                currPhysicsEventInfo = m_SelfPhysicsEventInfo;
                physicsEventInfo = m_CurrEvent.selfPhysicsEventInfo;
            }

            DrawField(() => { return currPhysicsEventInfo.force != physicsEventInfo.force; },
                () => { currPhysicsEventInfo.force = EditorGUILayout.Vector2Field("附加力", currPhysicsEventInfo.force); },
                () => { physicsEventInfo.force = currPhysicsEventInfo.force; }, 40);

            DrawField(() => { return currPhysicsEventInfo.velocity != physicsEventInfo.velocity; },
                () => { currPhysicsEventInfo.velocity = EditorGUILayout.Vector2Field("速度", currPhysicsEventInfo.velocity); },
                () => { physicsEventInfo.velocity = currPhysicsEventInfo.velocity; }, 40);

            DrawField(() => { return currPhysicsEventInfo.drag != physicsEventInfo.drag; },
                () => { currPhysicsEventInfo.drag = EditorGUILayout.FloatField("摩擦力", currPhysicsEventInfo.drag); },
                () => { physicsEventInfo.drag = currPhysicsEventInfo.drag; }, 20);

            DrawField(() => { return currPhysicsEventInfo.gravity != physicsEventInfo.gravity; },
                () => { currPhysicsEventInfo.gravity = EditorGUILayout.FloatField("重力", currPhysicsEventInfo.gravity); },
                () => { physicsEventInfo.gravity = currPhysicsEventInfo.gravity; }, 20);

            DrawField(() => { return currPhysicsEventInfo.distanceLimit != physicsEventInfo.distanceLimit; },
                () => { currPhysicsEventInfo.distanceLimit = EditorGUILayout.FloatField("距离限制", currPhysicsEventInfo.distanceLimit); },
                () => { physicsEventInfo.distanceLimit = currPhysicsEventInfo.distanceLimit; }, 20);
        }

        protected override void OnResetEvent()
        {
            base.ResetEvent();

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetPhysicsEvent)
            {
                m_CurrEvent.targetPhysicsEventInfo = null;
            }
            else if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfPhysicsEvent)
            {
                m_CurrEvent.selfPhysicsEventInfo = null;
            }
        }

        private PhysicsEventInfo m_TargetPhysicsEventInfo = null;
        private PhysicsEventInfo m_SelfPhysicsEventInfo = null;
    }
}