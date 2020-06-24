using FrameWork;
using FrameWork.BehaviorTree;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class FrameWorkEditorMgr
{
	[MenuItem("Tools/UI/创建UI场景", false, 3)]
	public static void NewUIScene()
	{
		UIUtility.NewUIScene();
	}

	public static string GetHierarchy(GameObject obj)
	{
		if (obj == null) return "";
		string path = obj.name;

		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			path = obj.name + "\\" + path;
		}
		return path;
	}

	public static void RegisterUndo(UnityEngine.Object obj, string name)
	{
#if UNITY_EDITOR
		UnityEditor.Undo.RecordObject(obj, name);
		if (obj)
		{
			UnityEditor.EditorUtility.SetDirty(obj);
		}
#endif
	}

	public static SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		SerializedProperty sp = serializedObject.FindProperty(property);

		if (sp != null)
		{
			if (sp.isArray && sp.type != "string") DrawArray(serializedObject, property, label ?? property);
			else if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
			else EditorGUILayout.PropertyField(sp, options);
		}
		else Debug.LogWarning("Unable to find property " + property);

		return sp;
	}

	public static void DrawArray(this SerializedObject obj, string property, string title)
	{
		SerializedProperty sp = obj.FindProperty(property + ".Array.size");

		if (sp != null && DrawHeader(title, title, false))
		{
			BeginContents();
			int size = sp.intValue;
			int newSize = EditorGUILayout.IntField("Size", size);
			if (newSize != size) obj.FindProperty(property + ".Array.size").intValue = newSize;

			EditorGUI.indentLevel = 1;

			for (int i = 0; i < newSize; i++)
			{
				SerializedProperty p = obj.FindProperty(string.Format("{0}.Array.data[{1}]", property, i));
				if (p != null) EditorGUILayout.PropertyField(p);
			}
			EditorGUI.indentLevel = 0;
			EndContents();
		}
	}

	public static bool DrawHeader(string text, string key, bool forceOn)
	{
		bool state = EditorPrefs.GetBool(key, true);

		GUILayout.Space(3f);
		if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;

		text = "<b><size=11>" + text + "</size></b>";
		if (state) text = "\u25BC " + text;
		else text = "\u25BA " + text;
		if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;

		if (GUI.changed) EditorPrefs.SetBool(key, state);

		GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!forceOn && !state) GUILayout.Space(3f);
		return state;
	}

	public static void BeginContents()
	{
		m_EndHorizontal = true;
		GUILayout.BeginHorizontal();
		EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));

		GUILayout.BeginVertical();
		GUILayout.Space(2f);
	}

	public static void EndContents()
	{
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (m_EndHorizontal)
		{
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
		}

		GUILayout.Space(3f);
	}


	[MenuItem("Assets/Config/CreateBehaviorTreeConfigData")]
	public static void CreateBehaviorConfig()
	{
		FrameWork.Utils.Utils.CreateConfigData<BehaviorTreeConfig, BehaviorTreeData>("BehaviorTreeConfigData", ".asset");
	}

	[MenuItem("GameObject/UI/EmojiText", priority = 0)]
	public static void CreateEmojiText()
	{
		//GameObject go = new GameObject("EmojiText", typeof(FrameWork.UI.EmojiText));
		//go.transform.SetParent(Selection.activeTransform, false);
		//go.GetComponent<EmojiText>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/UGUIEmoji");
		//FrameWork.Utils.Utils.CreateConfigData<BehaviorTreeConfig, BehaviorTreeData>("BehaviorTreeConfigData", ".asset");
	}

	private static bool m_EndHorizontal = false;
}
