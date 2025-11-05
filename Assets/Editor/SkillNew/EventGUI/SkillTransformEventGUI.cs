using UnityEditor;
using UnityEngine;
using static SkillEditorConfigData.SkillEvent;

namespace SkillNew
{
    public class SkillTransformEventGUI : SkillEventGUI
    {
        private TransformEventInfo m_TargetTransformInfo;
        private TransformEventInfo m_SelfTransformInfo;
        
        public SkillTransformEventGUI(EditorWindow window) : base(window)
        {
            m_TargetTransformInfo = new TransformEventInfo();
            m_SelfTransformInfo = new TransformEventInfo();
        }

        protected override void OnUpdateSkillEvent()
        {
            TransformEventInfo currTransformEventInfo = null;
            TransformEventInfo transformEventInfo = null;

            if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                currEvent.targetTransformEventInfo ??= new();
                m_TargetTransformInfo ??= new();
                currTransformEventInfo = m_TargetTransformInfo;
                transformEventInfo = currEvent.targetTransformEventInfo;
            }
            else if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                currEvent.selfTransformEventInfo ??= new TransformEventInfo();
                m_SelfTransformInfo ??= new();
                currTransformEventInfo = m_SelfTransformInfo;
                transformEventInfo = currEvent.selfTransformEventInfo;
            }

            if (currTransformEventInfo != null)
            {
                currTransformEventInfo.position = transformEventInfo.position;
                currTransformEventInfo.rotation = transformEventInfo.rotation;
                currTransformEventInfo.scale = transformEventInfo.scale;
                currTransformEventInfo.isPositionBasedOnSelf = transformEventInfo.isPositionBasedOnSelf;
                currTransformEventInfo.isRotationBasedOnSelf = transformEventInfo.isRotationBasedOnSelf;
                currTransformEventInfo.isPositionAnim = transformEventInfo.isPositionAnim;
                currTransformEventInfo.positionTweenInfo.duration = transformEventInfo.positionTweenInfo.duration;
                currTransformEventInfo.positionTweenInfo.delay = transformEventInfo.positionTweenInfo.delay;
                currTransformEventInfo.positionTweenInfo.ease = transformEventInfo.positionTweenInfo.ease;
                currTransformEventInfo.isRotationAnim = transformEventInfo.isRotationAnim;
                currTransformEventInfo.rotationTweenInfo.duration = transformEventInfo.rotationTweenInfo.duration;
                currTransformEventInfo.rotationTweenInfo.delay = transformEventInfo.rotationTweenInfo.delay;
                currTransformEventInfo.rotationTweenInfo.ease = transformEventInfo.rotationTweenInfo.ease;
                currTransformEventInfo.rotateMode = transformEventInfo.rotateMode;
                currTransformEventInfo.isScaleAnim = transformEventInfo.isScaleAnim;
                currTransformEventInfo.scaleTweenInfo.duration = transformEventInfo.scaleTweenInfo.duration;
                currTransformEventInfo.scaleTweenInfo.delay = transformEventInfo.scaleTweenInfo.delay;
                currTransformEventInfo.scaleTweenInfo.ease = transformEventInfo.scaleTweenInfo.ease;
            }
        }
        
        protected override void OnGUI()
        {
            TransformEventInfo currTransformInfo = null;
            TransformEventInfo transformEventInfo = null;

            if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.TargetTransformEvent)
            {
                currTransformInfo = m_TargetTransformInfo;
                transformEventInfo = currEvent.targetTransformEventInfo;
            }
            else if (currEvent.skillEventType == SkillEditorConfigData.SkillEventType.SelfTransformEvent)
            {
                currTransformInfo = m_SelfTransformInfo;
                transformEventInfo = currEvent.selfTransformEventInfo;
            }

            if (currTransformInfo == null || transformEventInfo == null)
            {
                return;
            }
            
            DrawField(() => { return currTransformInfo.position != transformEventInfo.position; },
                      () => { currTransformInfo.position = EditorGUILayout.Vector2Field("目标位置", currTransformInfo.position); },
                      () => { transformEventInfo.position = currTransformInfo.position; }, 40);

            DrawField(() => { return currTransformInfo.rotation != transformEventInfo.rotation; },
                      () => { currTransformInfo.rotation = EditorGUILayout.Vector3Field("目标旋转", currTransformInfo.rotation); },
                      () => { transformEventInfo.rotation = currTransformInfo.rotation; }, 40);

            DrawField(() => { return currTransformInfo.scale != transformEventInfo.scale; },
                      () => { currTransformInfo.scale = EditorGUILayout.Vector3Field("目标缩放", currTransformInfo.scale); },
                      () => { transformEventInfo.scale = currTransformInfo.scale; }, 40);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawField(() => { return currTransformInfo.isPositionBasedOnSelf != transformEventInfo.isPositionBasedOnSelf; },
                      () => { currTransformInfo.isPositionBasedOnSelf = EditorGUILayout.Toggle("基于自身位置", currTransformInfo.isPositionBasedOnSelf); },
                      () => { transformEventInfo.isPositionBasedOnSelf = currTransformInfo.isPositionBasedOnSelf; });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawField(() => { return currTransformInfo.isRotationBasedOnSelf != transformEventInfo.isRotationBasedOnSelf; },
                      () => { currTransformInfo.isRotationBasedOnSelf = EditorGUILayout.Toggle("基于自身旋转", currTransformInfo.isRotationBasedOnSelf); },
                      () => { transformEventInfo.isRotationBasedOnSelf = currTransformInfo.isRotationBasedOnSelf; });
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawAnimInfo(currTransformInfo, transformEventInfo, 1);
            DrawAnimInfo(currTransformInfo, transformEventInfo, 2);
            DrawAnimInfo(currTransformInfo, transformEventInfo, 3);
        }

        private void DrawAnimInfo(TransformEventInfo currTransformEventInfo, TransformEventInfo transformEventInfo, int type)
        {
            bool eventCondition = false;
            TweenInfo tweenInfo = null;
            TweenInfo eventTweenInfo = null;

            if (type == 1)
            {
                tweenInfo = currTransformEventInfo.positionTweenInfo;
                eventTweenInfo = transformEventInfo.positionTweenInfo;
                eventCondition = transformEventInfo.isPositionAnim;

                DrawField(() => { return currTransformEventInfo.isPositionAnim != transformEventInfo.isPositionAnim; },
                     () => { currTransformEventInfo.isPositionAnim = EditorGUILayout.Toggle("启用坐标动画补间", currTransformEventInfo.isPositionAnim); },
                     () => { transformEventInfo.isPositionAnim = currTransformEventInfo.isPositionAnim; }, 20);
            }
            else if (type == 2)
            {
                tweenInfo = currTransformEventInfo.rotationTweenInfo;
                eventTweenInfo = transformEventInfo.rotationTweenInfo;
                eventCondition = transformEventInfo.isRotationAnim;

                DrawField(() => { return currTransformEventInfo.isRotationAnim != transformEventInfo.isRotationAnim; },
                     () => { currTransformEventInfo.isRotationAnim = EditorGUILayout.Toggle("启用旋转动画补间", currTransformEventInfo.isRotationAnim); },
                     () => { transformEventInfo.isRotationAnim = currTransformEventInfo.isRotationAnim; }, 20);
            }
            else if (type == 3)
            {
                tweenInfo = currTransformEventInfo.scaleTweenInfo;
                eventTweenInfo = transformEventInfo.scaleTweenInfo;
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
                    DrawField(() => { return tweenInfo.duration != eventTweenInfo.duration; },
                       () => { tweenInfo.duration = EditorGUILayout.FloatField("动画时长", tweenInfo.duration); },
                       () => { eventTweenInfo.duration = tweenInfo.duration; }, 20);

                    DrawField(() => { return tweenInfo.delay != transformEventInfo.positionTweenInfo.delay; },
                       () => { tweenInfo.delay = EditorGUILayout.FloatField("动画延迟", tweenInfo.delay); },
                       () => { eventTweenInfo.delay = tweenInfo.delay; }, 20);

                    DrawField(() => { return tweenInfo.ease != eventTweenInfo.ease; },
                       () => { tweenInfo.ease = (DG.Tweening.Ease)EditorGUILayout.EnumPopup("动画曲线", tweenInfo.ease); },
                       () => { eventTweenInfo.ease = tweenInfo.ease; }, 20);

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
                if (tweenInfo != null)
                {
                    tweenInfo.duration = 0;
                    tweenInfo.delay = 0;
                    tweenInfo.ease = DG.Tweening.Ease.Unset;
                }

                if (eventTweenInfo != null)
                {
                    eventTweenInfo.duration = 0;
                    eventTweenInfo.delay = 0;
                    eventTweenInfo.ease = DG.Tweening.Ease.Unset;
                }
            }
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        
        protected override void OnResetEvent()
        {
            m_TargetTransformInfo = null;
            m_SelfTransformInfo = null;
        }
    }
}