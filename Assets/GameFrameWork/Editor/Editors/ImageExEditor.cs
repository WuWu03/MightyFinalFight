using GameFrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [CustomEditor(typeof(ImageEx))]
    public class ImageExEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            m_Image = (target as ImageEx);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SerializedProperty scores = serializedObject.FindProperty("scores");
            SerializedProperty maxScore = serializedObject.FindProperty("maxScore");

            if (m_Image.useRadar)
            {
                if (m_Image.scores.Length > m_Image.segements)
                {
                    m_Image.scores = new int[m_Image.segements];
                }

                for (int i = 0; i < m_Image.scores.Length; i++)
                {
                    int score = scores.GetArrayElementAtIndex(i).intValue;
                    score = Mathf.Max(0, score);
                    m_Image.scores[i] = Mathf.Min(score, maxScore.intValue);
                }
            }
            else
            {
                m_Image.scores = null;
            }
        }

        private ImageEx m_Image;
    }
}