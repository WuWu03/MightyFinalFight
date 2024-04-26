using NUnit.Framework.Interfaces;
using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillTransformEventGUI : SkillEventGUI
    {
        public SkillTransformEventGUI(EditorWindow window) : base(window)
        {
            m_TargetTransformInfo = new TransformEventInfo();
            m_SelfTransformInfo = new TransformEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            TransformEventInfo transformInfo = null;
            TransformEventInfo eventTransformInfo = null;

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                if(m_CurrEvent.targetTransformEventInfo == null)
                {
                    m_CurrEvent.targetTransformEventInfo = new TransformEventInfo();
                }

                transformInfo = m_TargetTransformInfo;
                eventTransformInfo = m_CurrEvent.targetTransformEventInfo;
            }
            else if(m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                if (m_CurrEvent.selfTransformEventInfo == null)
                {
                    m_CurrEvent.selfTransformEventInfo = new TransformEventInfo();
                }

                transformInfo = m_SelfTransformInfo;
                eventTransformInfo = m_CurrEvent.selfTransformEventInfo;
            }

            transformInfo.position = eventTransformInfo.position;
            transformInfo.rotation = eventTransformInfo.rotation;
            transformInfo.scale = eventTransformInfo.scale;
            transformInfo.isPositionBasedOnSelf = eventTransformInfo.isPositionBasedOnSelf;
            transformInfo.isRotationBasedOnSelf = eventTransformInfo.isRotationBasedOnSelf;

            transformInfo.isPositionAnim = eventTransformInfo.isPositionAnim;
            transformInfo.positionAnimInfo.duration = eventTransformInfo.positionAnimInfo.duration;
            transformInfo.positionAnimInfo.delay = eventTransformInfo.positionAnimInfo.delay;
            transformInfo.positionAnimInfo.ease = eventTransformInfo.positionAnimInfo.ease;

            transformInfo.isRotationAnim = eventTransformInfo.isRotationAnim;
            transformInfo.rotationAnimInfo.duration = eventTransformInfo.rotationAnimInfo.duration;
            transformInfo.rotationAnimInfo.delay = eventTransformInfo.rotationAnimInfo.delay;
            transformInfo.rotationAnimInfo.ease = eventTransformInfo.rotationAnimInfo.ease;
            transformInfo.rotateMode = eventTransformInfo.rotateMode;

            transformInfo.isScaleAnim = eventTransformInfo.isScaleAnim;
            transformInfo.scaleAnimInfo.duration = eventTransformInfo.scaleAnimInfo.duration;
            transformInfo.scaleAnimInfo.delay = eventTransformInfo.scaleAnimInfo.delay;
            transformInfo.scaleAnimInfo.ease = eventTransformInfo.scaleAnimInfo.ease;
        }

        public override void ResetEvent()
        {
            base.ResetEvent();

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                m_CurrEvent.targetTransformEventInfo = null;
            }
            else if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                m_CurrEvent.selfTransformEventInfo = null;
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (m_CurrEvent == null)
            {
                return;
            }

            TransformEventInfo transformInfo = null;
            TransformEventInfo eventTransformInfo = null;

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                transformInfo = m_TargetTransformInfo;
                eventTransformInfo = m_CurrEvent.targetTransformEventInfo;
            }
            else if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                transformInfo = m_SelfTransformInfo;
                eventTransformInfo = m_CurrEvent.selfTransformEventInfo;
            }

            if (transformInfo == null || eventTransformInfo == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();

            DrawField(() => { return transformInfo.position != eventTransformInfo.position; },
                      () => { transformInfo.position = EditorGUILayout.Vector2Field("目标位置", transformInfo.position); },
                      () => { eventTransformInfo.position = transformInfo.position; }, 40);

            DrawField(() => { return transformInfo.rotation != eventTransformInfo.rotation; },
                      () => { transformInfo.rotation = EditorGUILayout.Vector3Field("目标旋转", transformInfo.rotation); },
                      () => { eventTransformInfo.rotation = transformInfo.rotation; }, 40);

            DrawField(() => { return transformInfo.scale != eventTransformInfo.scale; },
                      () => { transformInfo.scale = EditorGUILayout.Vector3Field("目标缩放", transformInfo.scale); },
                      () => { eventTransformInfo.scale = transformInfo.scale; }, 40);

            DrawField(() => { return transformInfo.isPositionBasedOnSelf != eventTransformInfo.isPositionBasedOnSelf; },
                      () => { transformInfo.isPositionBasedOnSelf = EditorGUILayout.Toggle("基于自身位置", transformInfo.isPositionBasedOnSelf); },
                      () => { eventTransformInfo.isPositionBasedOnSelf = transformInfo.isPositionBasedOnSelf; }, 20);

            DrawField(() => { return transformInfo.isRotationBasedOnSelf != eventTransformInfo.isRotationBasedOnSelf; },
                      () => { transformInfo.isRotationBasedOnSelf = EditorGUILayout.Toggle("基于自身旋转", transformInfo.isRotationBasedOnSelf); },
                      () => { eventTransformInfo.isRotationBasedOnSelf = transformInfo.isRotationBasedOnSelf; }, 20);

            DrawAnimInfo(transformInfo, eventTransformInfo, 1);
            DrawAnimInfo(transformInfo, eventTransformInfo, 2);
            DrawAnimInfo(transformInfo, eventTransformInfo, 3);

            EditorGUILayout.EndVertical();
        }

        private void DrawAnimInfo(TransformEventInfo transformInfo, TransformEventInfo eventTransformInfo,int type)
        {
            bool condition = false;
            bool eventCondition = false;
            AnimInfo animInfo = null;
            AnimInfo eventAnimInfo = null;

            if(type == 1)
            {
                animInfo = transformInfo.positionAnimInfo;
                eventAnimInfo = eventTransformInfo.positionAnimInfo;
                condition = transformInfo.isPositionAnim;
                eventCondition = eventTransformInfo.isPositionAnim;

                DrawField(() => { return transformInfo.isPositionAnim != eventTransformInfo.isPositionAnim; },
                     () => { transformInfo.isPositionAnim = EditorGUILayout.Toggle("启用坐标动画补间", transformInfo.isPositionAnim); },
                     () => { eventTransformInfo.isPositionAnim = transformInfo.isPositionAnim; }, 20);
            }
            else if(type == 2)
            {
                animInfo = transformInfo.rotationAnimInfo;
                eventAnimInfo = eventTransformInfo.rotationAnimInfo;
                condition = transformInfo.isRotationAnim;
                eventCondition = eventTransformInfo.isRotationAnim;

                DrawField(() => { return transformInfo.isRotationAnim != eventTransformInfo.isRotationAnim; },
                     () => { transformInfo.isRotationAnim = EditorGUILayout.Toggle("启用旋转动画补间", transformInfo.isRotationAnim); },
                     () => { eventTransformInfo.isRotationAnim = transformInfo.isRotationAnim; }, 20);
            }
            else if(type == 3)
            {
                animInfo = transformInfo.scaleAnimInfo;
                eventAnimInfo = eventTransformInfo.scaleAnimInfo;
                condition = transformInfo.isScaleAnim;
                eventCondition = eventTransformInfo.isScaleAnim;

                DrawField(() => { return transformInfo.isScaleAnim != eventTransformInfo.isScaleAnim; },
                     () => { transformInfo.isScaleAnim = EditorGUILayout.Toggle("启用缩放动画补间", transformInfo.isScaleAnim); },
                     () => { eventTransformInfo.isScaleAnim = transformInfo.isScaleAnim; }, 20);
            }

            if (eventCondition)
            {
                GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                {
                    EditorGUILayout.BeginVertical();
                    DrawField(() => { return animInfo.duration != eventAnimInfo.duration; },
                       () => { animInfo.duration = EditorGUILayout.FloatField("动画时长", animInfo.duration); },
                       () => { eventAnimInfo.duration = animInfo.duration; }, 20);

                    DrawField(() => { return animInfo.delay != eventTransformInfo.positionAnimInfo.delay; },
                       () => { animInfo.delay = EditorGUILayout.FloatField("动画延迟", animInfo.delay); },
                       () => { eventAnimInfo.delay = animInfo.delay; }, 20);

                    DrawField(() => { return animInfo.ease != eventAnimInfo.ease; },
                       () => { animInfo.ease = (DG.Tweening.Ease)EditorGUILayout.EnumPopup("动画曲线", animInfo.ease); },
                       () => { eventAnimInfo.ease = animInfo.ease; }, 20);

                    if (type == 2)
                    {
                        DrawField(() => { return transformInfo.rotateMode != eventTransformInfo.rotateMode; },
                         () => { transformInfo.rotateMode = (DG.Tweening.RotateMode)EditorGUILayout.EnumPopup("动画曲线", transformInfo.rotateMode); },
                         () => { eventTransformInfo.rotateMode = transformInfo.rotateMode; }, 20);
                    }
                    EditorGUILayout.EndVertical();
                });
            }
            else
            {
                animInfo.duration = 0;
                animInfo.delay = 0;
                animInfo.ease = DG.Tweening.Ease.Unset;
                eventAnimInfo.duration = 0;
                eventAnimInfo.delay = 0;
                eventAnimInfo.ease = DG.Tweening.Ease.Unset;
            }
        }

        private TransformEventInfo m_TargetTransformInfo = null;
        private TransformEventInfo m_SelfTransformInfo = null;
    }
}