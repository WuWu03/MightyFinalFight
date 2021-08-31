using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using GameFrameWork.Serialize;
using GameFrameWork.Utility;
using GameFrameWork.BehaviourTree;
using System.Reflection;

namespace GameFrameWork.Editor
{
	public static class EditorUtility
	{
		public static void CreateBehaviorConfig(string name, string extend, string path)
		{
			CreateConfigData<BehaviourTreeConfig, BehaviourTreeData>(name, extend, path);
		}

		public static void CreateConfigData<T, P>(string name, string ext, string dir = null) where T : BaseScriptableObject<P>
																							  where P : BaseConfigData
		{
			CreateScriptableObject(typeof(T), name, ext, dir);
		}

		public static void CreateScriptableObject<T>(string name, string ext, string dir = null) where T : ScriptableObject
		{
			CreateScriptableObject(typeof(T), name, ext, dir);
		}

		private static void CreateScriptableObject(Type type,string name, string ext, string dir = null)
        {
			string directory = PathUtil.ConfigDataDefaultFullPath;
			if (!string.IsNullOrEmpty(dir)) directory = dir;

			string fileName = directory + name + ext;
			if (File.Exists(fileName))
			{
				return;
			}

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			ScriptableObject data = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(data, directory.Substring(directory.IndexOf("Assets")) + name + ext);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void GUIBoxScope(System.Action action)
		{
			GUIStyle style1 = GUI.skin.label;
			GUIStyle style2 = GUI.skin.window;
			style2.padding.top = 0;
			style2.padding.bottom = 5;
			style2.stretchHeight = false;

			using (new GUILayout.VerticalScope(style1, new GUILayoutOption[0]))
			{
				using (new GUILayout.HorizontalScope(style2, new GUILayoutOption[0]))
				{
					action?.Invoke();
				}
			}
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

		/// <summary>
		/// 在枚举值加上EnumLabel标签可以显示自定义名字
		/// </summary>
		public static object EnumPopup(string title, Enum selected)
		{
			int index = 0;
			var array = Enum.GetValues(selected.GetType());
			int length = array.Length;

			string[] enumString = new string[length];
			for (int i = 0; i < length; i++)
			{
				FieldInfo[] fields = selected.GetType().GetFields();
				foreach (FieldInfo field in fields)
				{
					if (field.Name.Equals(array.GetValue(i).ToString()))
					{
						object[] objs = field.GetCustomAttributes(typeof(EnumLabelAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							enumString[i] = ((EnumLabelAttribute)objs[0]).label;
						}
					}
				}
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(title);
			index = EditorGUILayout.Popup(selected.GetHashCode(), enumString);
			EditorGUILayout.EndHorizontal();

			return Enum.ToObject(selected.GetType(), index);
		}

		public static void DrawCurve(Rect start, Rect end, Color color)
		{
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
			Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
			Vector3 startTan = startPos + Vector3.right * 50;
			Vector3 endTan = endPos + Vector3.left * 50;
			Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 4);
		}


		private static bool m_EndHorizontal = false;
	}
}