using GameFrameWork.BehaviourTree;
using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
	public static class EditorUtil
	{
		public static string[] GetAssemblyTypeNames(string typeName, bool isFullName, params string[] parttern)
		{
			Type[] types = GetAssemblyTypes(typeName, parttern);

			string[] typeNames = new string[types.Length];

			for (int i = 0; i < types.Length; i++)
			{
				typeNames[i] = isFullName ? types[i].FullName : types[i].Name;
			}

			return typeNames;
		}

		public static Type[] GetAssemblyTypes(string typeName, params string[] parttern)
		{
			Assembly assembly = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			Type[] allTypes = assembly.GetTypes();

			List<Type> list = new List<Type>();
			Type baseType = assembly.GetType(typeName);

			foreach (Type type in allTypes)
			{
				Type temp = type;
				while (temp.BaseType != null)
				{
					if (temp.Name.Equals(baseType.Name))
					{
						bool isParttern = false;

						for (int i = 0; i < parttern.Length; i++)
						{
							if (parttern[i].Equals(type.Name))
							{
								isParttern = true;
								break;
							}
						}

						if (!isParttern)
						{
							list.Add(type);
							break;
						}
					}

					temp = temp.BaseType;
				}
			}

			list.Sort((a, b) =>
			{
				return a.Name.CompareTo(b.Name);
			});

			return list.ToArray();
		}

		public static string GetNodePath(Transform current, params string[] endParttern)
		{
			return GetNodePath(current, string.Empty, endParttern);
		}

		private static string GetNodePath(Transform current, string path, params string[] endParttern)
		{
			if (current == null)
			{
				return path;
			}

			if (string.IsNullOrEmpty(path))
			{
				path = current.name;
			}
			else
			{
				path = current.name + "/" + path;
			}

			if (current.parent != null && endParttern != null)
			{
				for (int i = 0; i < endParttern.Length; i++)
				{
					if (current.parent.name.Contains(endParttern[i]))
					{
						return path;
					}
				}
			}

			return GetNodePath(current.parent, path, endParttern);
		}

		public static void CreateConfigData<T, P>(string name, string ext, string dir = null) where T : BaseScriptableObject<P> where P : BaseConfigData
		{
			CreateScriptableObject(typeof(T), name, ext, dir);
		}

		public static void CreateScriptableObject<T>(string name, string ext, string dir = null) where T : ScriptableObject
		{
			CreateScriptableObject(typeof(T), name, ext, dir);
		}

		private static void CreateScriptableObject(Type type, string name, string ext, string dir = null)
		{
			string directory = EditorPathUtil.configDataFullPath;
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
			UnityEditor.Undo.RecordObject(obj, name);

			if (obj != null)
			{
				UnityEditor.EditorUtility.SetDirty(obj);
			}
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
			else UnityEngine.Debug.LogWarning("Unable to find property " + property);

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

				if (newSize != size)
				{
					obj.FindProperty(property + ".Array.size").intValue = newSize;
				}

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

		public static void CopyTextEditor(string content)
		{
			TextEditor editor = new TextEditor();
			editor.text = content;
			editor.SelectAll();
			editor.Copy();
			UnityEditor.EditorUtility.DisplayDialog("提示", "路径已复制到剪切板", "确定");
		}

		public static void AddMenuItem(string menuItem, System.Action callback, int priority = -1)
		{
			typeof(Menu).GetMethod("AddMenuItem", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { menuItem, null, null, priority, callback, null });
		}

		public static void RemoveMenuItem(string menuItem)
		{
			typeof(Menu).GetMethod("RemoveMenuItem", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { menuItem });
		}

		public static void RemoveAllMenuItem()
		{
			typeof(Menu).GetMethod("RebuildAllMenus", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        }

		private static bool m_EndHorizontal = false;
	}
}