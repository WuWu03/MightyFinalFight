using UnityEditor;
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
            TransformEventInfo currTransformEventInfo = null;
            TransformEventInfo transformEventInfo = null;

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                if(m_CurrEvent.targetTransformEventInfo == null)
                {
                    m_CurrEvent.targetTransformEventInfo = new TransformEventInfo();
                }

                currTransformEventInfo = m_TargetTransformInfo;
                transformEventInfo = m_CurrEvent.targetTransformEventInfo;
            }
            else if(m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                if (m_CurrEvent.selfTransformEventInfo == null)
                {
                    m_CurrEvent.selfTransformEventInfo = new TransformEventInfo();
                }

                currTransformEventInfo = m_SelfTransformInfo;
                transformEventInfo = m_CurrEvent.selfTransformEventInfo;
            }

            currTransformEventInfo.position = transformEventInfo.position;
            currTransformEventInfo.rotation = transformEventInfo.rotation;
            currTransformEventInfo.scale = transformEventInfo.scale;
            currTransformEventInfo.isPositionBasedOnSelf = transformEventInfo.isPositionBasedOnSelf;
            currTransformEventInfo.isRotationBasedOnSelf = transformEventInfo.isRotationBasedOnSelf;

            currTransformEventInfo.isPositionAnim = transformEventInfo.isPositionAnim;
            currTransformEventInfo.positionAnimInfo.duration = transformEventInfo.positionAnimInfo.duration;
            currTransformEventInfo.positionAnimInfo.delay = transformEventInfo.positionAnimInfo.delay;
            currTransformEventInfo.positionAnimInfo.ease = transformEventInfo.positionAnimInfo.ease;

            currTransformEventInfo.isRotationAnim = transformEventInfo.isRotationAnim;
            currTransformEventInfo.rotationAnimInfo.duration = transformEventInfo.rotationAnimInfo.duration;
            currTransformEventInfo.rotationAnimInfo.delay = transformEventInfo.rotationAnimInfo.delay;
            currTransformEventInfo.rotationAnimInfo.ease = transformEventInfo.rotationAnimInfo.ease;
            currTransformEventInfo.rotateMode = transformEventInfo.rotateMode;

            currTransformEventInfo.isScaleAnim = transformEventInfo.isScaleAnim;
            currTransformEventInfo.scaleAnimInfo.duration = transformEventInfo.scaleAnimInfo.duration;
            currTransformEventInfo.scaleAnimInfo.delay = transformEventInfo.scaleAnimInfo.delay;
            currTransformEventInfo.scaleAnimInfo.ease = transformEventInfo.scaleAnimInfo.ease;
        }

        protected override void OnResetEvent()
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

            TransformEventInfo currTransformInfo = null;
            TransformEventInfo transformEventInfo = null;

            if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                currTransformInfo = m_TargetTransformInfo;
                transformEventInfo = m_CurrEvent.targetTransformEventInfo;
            }
            else if (m_CurrEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                currTransformInfo = m_SelfTransformInfo;
                transformEventInfo = m_CurrEvent.selfTransformEventInfo;
            }

            if (currTransformInfo == null || transformEventInfo == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical();

            DrawField(() => { return currTransformInfo.position != transformEventInfo.position; },
                      () => { currTransformInfo.position = EditorGUILayout.Vector2Field("目标位置", currTransformInfo.position); },
                      () => { transformEventInfo.position = currTransformInfo.position; }, 40);

            DrawField(() => { return currTransformInfo.rotation != transformEventInfo.rotation; },
                      () => { currTransformInfo.rotation = EditorGUILayout.Vector3Field("目标旋转", currTransformInfo.rotation); },
                      () => { transformEventInfo.rotation = currTransformInfo.rotation; }, 40);

            DrawField(() => { return currTransformInfo.scale != transformEventInfo.scale; },
                      () => { currTransformInfo.scale = EditorGUILayout.Vector3Field("目标缩放", currTransformInfo.scale); },
                      () => { transformEventInfo.scale = currTransformInfo.scale; }, 40);

            DrawField(() => { return currTransformInfo.isPositionBasedOnSelf != transformEventInfo.isPositionBasedOnSelf; },
                      () => { currTransformInfo.isPositionBasedOnSelf = EditorGUILayout.Toggle("基于自身位置", currTransformInfo.isPositionBasedOnSelf); },
                      () => { transformEventInfo.isPositionBasedOnSelf = currTransformInfo.isPositionBasedOnSelf; }, 20);

            DrawField(() => { return currTransformInfo.isRotationBasedOnSelf != transformEventInfo.isRotationBasedOnSelf; },
                      () => { currTransformInfo.isRotationBasedOnSelf = EditorGUILayout.Toggle("基于自身旋转", currTransformInfo.isRotationBasedOnSelf); },
                      () => { transformEventInfo.isRotationBasedOnSelf = currTransformInfo.isRotationBasedOnSelf; }, 20);

            DrawAnimInfo(currTransformInfo, transformEventInfo, 1);
            DrawAnimInfo(currTransformInfo, transformEventInfo, 2);
            DrawAnimInfo(currTransformInfo, transformEventInfo, 3);

            EditorGUILayout.EndVertical();
        }

        private void DrawAnimInfo(TransformEventInfo currTransformEventInfo, TransformEventInfo transformEventInfo, int type)
        {
            bool condition = false;
            bool eventCondition = false;
            AnimInfo animInfo = null;
            AnimInfo eventAnimInfo = null;

            if (type == 1)
            {
                animInfo = currTransformEventInfo.positionAnimInfo;
                eventAnimInfo = transformEventInfo.positionAnimInfo;
                condition = currTransformEventInfo.isPositionAnim;
                eventCondition = transformEventInfo.isPositionAnim;

                DrawField(() => { return currTransformEventInfo.isPositionAnim != transformEventInfo.isPositionAnim; },
                     () => { currTransformEventInfo.isPositionAnim = EditorGUILayout.Toggle("启用坐标动画补间", currTransformEventInfo.isPositionAnim); },
                     () => { transformEventInfo.isPositionAnim = currTransformEventInfo.isPositionAnim; }, 20);
            }
            else if (type == 2)
            {
                animInfo = currTransformEventInfo.rotationAnimInfo;
                eventAnimInfo = transformEventInfo.rotationAnimInfo;
                condition = currTransformEventInfo.isRotationAnim;
                eventCondition = transformEventInfo.isRotationAnim;

                DrawField(() => { return currTransformEventInfo.isRotationAnim != transformEventInfo.isRotationAnim; },
                     () => { currTransformEventInfo.isRotationAnim = EditorGUILayout.Toggle("启用旋转动画补间", currTransformEventInfo.isRotationAnim); },
                     () => { transformEventInfo.isRotationAnim = currTransformEventInfo.isRotationAnim; }, 20);
            }
            else if (type == 3)
            {
                animInfo = currTransformEventInfo.scaleAnimInfo;
                eventAnimInfo = transformEventInfo.scaleAnimInfo;
                condition = currTransformEventInfo.isScaleAnim;
                eventCondition = transformEventInfo.isScaleAnim;

                DrawField(() => { return currTransformEventInfo.isScaleAnim != transformEventInfo.isScaleAnim; },
                     () => { currTransformEventInfo.isScaleAnim = EditorGUILayout.Toggle("启用缩放动画补间", currTransformEventInfo.isScaleAnim); },
                     () => { transformEventInfo.isScaleAnim = currTransformEventInfo.isScaleAnim; }, 20);
            }

            if (eventCondition)
            {
                GameFrameWork.Editor.EditorUtil.GUIBoxScope(() =>
                {
                    EditorGUILayout.BeginVertical();
                    DrawField(() => { return animInfo.duration != eventAnimInfo.duration; },
                       () => { animInfo.duration = EditorGUILayout.FloatField("动画时长", animInfo.duration); },
                       () => { eventAnimInfo.duration = animInfo.duration; }, 20);

                    DrawField(() => { return animInfo.delay != transformEventInfo.positionAnimInfo.delay; },
                       () => { animInfo.delay = EditorGUILayout.FloatField("动画延迟", animInfo.delay); },
                       () => { eventAnimInfo.delay = animInfo.delay; }, 20);

                    DrawField(() => { return animInfo.ease != eventAnimInfo.ease; },
                       () => { animInfo.ease = (DG.Tweening.Ease)EditorGUILayout.EnumPopup("动画曲线", animInfo.ease); },
                       () => { eventAnimInfo.ease = animInfo.ease; }, 20);

                    if (type == 2)
                    {
                        DrawField(() => { return currTransformEventInfo.rotateMode != transformEventInfo.rotateMode; },
                         () => { currTransformEventInfo.rotateMode = (DG.Tweening.RotateMode)EditorGUILayout.EnumPopup("动画曲线", currTransformEventInfo.rotateMode); },
                         () => { transformEventInfo.rotateMode = currTransformEventInfo.rotateMode; }, 20);
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