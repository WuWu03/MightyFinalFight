// using GameFrameWork.UI;
// using UnityEditor;
// using UnityEngine.UI;
//
// namespace GameFrameWork.Editor
// {
//     [CustomEditor(typeof(ScrollList))]
//     public class ScrollListEditor : UnityEditor.Editor
//     {
//         private ScrollList m_ScrollList;
//         private ScrollRect m_ScrollRect;
//
//         private void OnEnable()
//         {
//             m_ScrollList = (ScrollList)target;
//             m_ScrollRect = m_ScrollList.GetComponent<ScrollRect>();
//         }
//
//         public override void OnInspectorGUI()
//         {
//             SerializedProperty isHorizontalReverse = serializedObject.FindProperty("isHorizontalReverse");
//             SerializedProperty isVerticalReverse = serializedObject.FindProperty("isVerticalReverse");
//             SerializedProperty xSpacing = serializedObject.FindProperty("xSpacing");
//             SerializedProperty ySpacing = serializedObject.FindProperty("ySpacing");
//             SerializedProperty prefab = serializedObject.FindProperty("prefab");
//             EditorGUILayout.PropertyField(isHorizontalReverse);
//             EditorGUILayout.PropertyField(isVerticalReverse);
//             EditorGUILayout.PropertyField(xSpacing);
//             EditorGUILayout.PropertyField(ySpacing);
//             EditorGUILayout.PropertyField(prefab);
//         }
//     }
// }