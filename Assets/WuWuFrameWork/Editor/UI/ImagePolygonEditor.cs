using System.Linq;
using WuWuFramework.UI;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

namespace WuWuFramework.Editor
{
    [CustomEditor(typeof(ImagePolygon))]
    public class ImagePolygonEditor : ImageEditor
    {
        private ImagePolygon m_ImagePolygon;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_ImagePolygon = target as ImagePolygon;
        }
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool isReRender = false;
            if (!m_ImagePolygon.hasBorder || (m_ImagePolygon.type == Image.Type.Simple || m_ImagePolygon.type == Image.Type.Filled))
            {
                SerializedProperty fillCenter = serializedObject.FindProperty("m_FillCenter");
                EditorGUILayout.PropertyField(fillCenter);
                isReRender = m_ImagePolygon.fillCenter != fillCenter.boolValue;
                m_ImagePolygon.fillCenter = fillCenter.boolValue;
            }
            
            SerializedProperty fillPercent = serializedObject.FindProperty("fillPercent");
            EditorGUILayout.PropertyField(fillPercent);
            isReRender = isReRender || !Mathf.Approximately(m_ImagePolygon.fillPercent, fillPercent.floatValue);
            m_ImagePolygon.fillPercent = fillPercent.floatValue;

            SerializedProperty segements = serializedObject.FindProperty("segements");
            EditorGUILayout.IntSlider(segements, 3, 50,"Segements");
            isReRender = isReRender || m_ImagePolygon.segements != segements.intValue;
            m_ImagePolygon.segements = segements.intValue;
            
            if (!m_ImagePolygon.fillCenter)
            {
                SerializedProperty thickness = serializedObject.FindProperty("thickness");
                EditorGUILayout.Slider(thickness, 0, m_ImagePolygon.rectTransform.sizeDelta.x / 2,"Thickness");
                isReRender = isReRender || !Mathf.Approximately(m_ImagePolygon.thickness, thickness.floatValue);
                m_ImagePolygon.thickness = thickness.floatValue;
                m_ImagePolygon.useRadar = false;
                m_ImagePolygon.scores = null;
            }
            else
            {
                SerializedProperty useRadar = serializedObject.FindProperty("useRadar");
                EditorGUILayout.PropertyField(useRadar);
                isReRender = isReRender || m_ImagePolygon.useRadar != useRadar.boolValue;
                m_ImagePolygon.useRadar = useRadar.boolValue;
                m_ImagePolygon.thickness = 5;
            }

            if (m_ImagePolygon.useRadar)
            {
                SerializedProperty maxScore = serializedObject.FindProperty("maxScore");
                EditorGUILayout.PropertyField(maxScore);
                int tempValue = Mathf.Abs(maxScore.intValue);
                isReRender = isReRender || m_ImagePolygon.maxScore != tempValue;
                m_ImagePolygon.maxScore = tempValue;
                m_ImagePolygon.scores ??= new int[m_ImagePolygon.segements];

                for (int i = 0; i < m_ImagePolygon.scores.Length; i++)
                {
                    int score = EditorGUILayout.IntSlider("Score" + (i + 1), m_ImagePolygon.scores[i], 0, m_ImagePolygon.maxScore);
                    isReRender = isReRender || m_ImagePolygon.scores[i] != score;
                    m_ImagePolygon.scores[i] = score;
                }
            }
            else
            {
                m_ImagePolygon.scores = null;
            }
            
            if (isReRender)
            {
                m_ImagePolygon.SetAllDirty();
                m_ImagePolygon.Rebuild(CanvasUpdate.PreRender);
            }
        }
    }
}
