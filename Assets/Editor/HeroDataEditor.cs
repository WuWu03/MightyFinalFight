using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(HeroConfig), true)]
public class HeroDataEditor : Editor
{
    HeroConfig m_Text;

    private void OnEnable()
    {
        m_Text = (target as HeroConfig);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //serializedObject.Update();
       
        for (int i = 0; i < m_Text.Datas.Length; i++)
        {
            EditorGUILayout.LabelField("Desc");
            GUILayout.FlexibleSpace();
            m_Text.Datas[i].Desc = EditorGUILayout.TextArea(m_Text.Datas[i].Desc);
        }


        serializedObject.ApplyModifiedProperties();
    }
}
