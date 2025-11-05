using DG.DemiEditor;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillPhysicsEventGUI : SkillEventGUI
    {
        private PhysicsEventInfo m_TargetPhysicsEventInfo;
        private PhysicsEventInfo m_SelfPhysicsEventInfo;

        public SkillPhysicsEventGUI(EditorWindow window) : base(window)
        {
            m_TargetPhysicsEventInfo = new();
            m_SelfPhysicsEventInfo = new();
        }

        protected override void OnUpdateSkillEvent()
        {
            PhysicsEventInfo currPhysicsEventInfo = null;
            PhysicsEventInfo physicsEventInfo = null;

            if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetPhysicsEvent)
            {
                currEvent.targetPhysicsEventInfo ??= new();
                m_TargetPhysicsEventInfo ??= new();
                currPhysicsEventInfo = m_TargetPhysicsEventInfo;
                physicsEventInfo = currEvent.targetPhysicsEventInfo;
            }
            else if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfPhysicsEvent)
            {
                currEvent.selfPhysicsEventInfo ??= new();
                m_SelfPhysicsEventInfo ??= new();
                currPhysicsEventInfo = m_SelfPhysicsEventInfo;
                physicsEventInfo = currEvent.selfPhysicsEventInfo;
            }

            if (currPhysicsEventInfo != null)
            {
                currPhysicsEventInfo.groundForceInfo = physicsEventInfo.groundForceInfo;
                currPhysicsEventInfo.floatForceInfo = physicsEventInfo.floatForceInfo;
                currPhysicsEventInfo.lieGroundForceInfo = physicsEventInfo.lieGroundForceInfo;
            }
        }

        protected override void OnGUI()
        {
            PhysicsEventInfo currPhysicsEventInfo = null;
            PhysicsEventInfo physicsEventInfo = null;

            if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetPhysicsEvent)
            {
                currPhysicsEventInfo = m_TargetPhysicsEventInfo;
                physicsEventInfo = currEvent.targetPhysicsEventInfo;
            }
            else if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfPhysicsEvent)
            {
                currPhysicsEventInfo = m_SelfPhysicsEventInfo;
                physicsEventInfo = currEvent.selfPhysicsEventInfo;
            }

            if (currPhysicsEventInfo != null)
            {
                GUIStyle style = new("sv_label_0")
                {
                    alignment = TextAnchor.MiddleCenter,
                };
                EditorGUILayout.LabelField("一般", style);
                Draw(currPhysicsEventInfo.groundForceInfo, physicsEventInfo.groundForceInfo);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("浮空", style);
                Draw(currPhysicsEventInfo.floatForceInfo, physicsEventInfo.floatForceInfo);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("扫地", style);
                Draw(currPhysicsEventInfo.lieGroundForceInfo, physicsEventInfo.lieGroundForceInfo);
            }
        }

        private void Draw(ForceInfo currForceInfo, ForceInfo forceInfo)
        {
            DrawField(() => { return currForceInfo.force != forceInfo.force; },
                () => { currForceInfo.force = EditorGUILayout.Vector2Field("附加力", forceInfo.force); },
                () => { forceInfo.force = currForceInfo.force; }, 40);

            DrawField(() => { return currForceInfo.velocity != forceInfo.velocity; },
                () => { currForceInfo.velocity = EditorGUILayout.Vector2Field("速度", currForceInfo.velocity); },
                () => { forceInfo.velocity = currForceInfo.velocity; }, 40);

            DrawField(() => { return !Mathf.Approximately(currForceInfo.drag, forceInfo.drag); },
                () => { currForceInfo.drag = EditorGUILayout.FloatField("摩擦力", currForceInfo.drag); },
                () => { forceInfo.drag = currForceInfo.drag; });

            DrawField(() => { return !Mathf.Approximately(currForceInfo.gravity, forceInfo.gravity); },
                () => { currForceInfo.gravity = EditorGUILayout.FloatField("重力", currForceInfo.gravity); },
                () => { forceInfo.gravity = currForceInfo.gravity; });

            DrawField(() => { return !Mathf.Approximately(currForceInfo.distanceLimit, forceInfo.distanceLimit); },
                () =>
                {
                    currForceInfo.distanceLimit = EditorGUILayout.FloatField("距离限制", currForceInfo.distanceLimit);
                },
                () => { forceInfo.distanceLimit = currForceInfo.distanceLimit; });
        }

        protected override void OnResetEvent()
        {
            if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetPhysicsEvent)
            {
                currEvent.targetPhysicsEventInfo = null;
            }
            else if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfPhysicsEvent)
            {
                currEvent.selfPhysicsEventInfo = null;
            }
        }
    }
}