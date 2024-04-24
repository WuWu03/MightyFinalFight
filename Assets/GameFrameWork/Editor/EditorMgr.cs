using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
	public static class EditorMgr
	{
		static EditorMgr()
		{
			if (!Directory.Exists(EditorPathUtil.configDataFullPath))
			{
				Directory.CreateDirectory(EditorPathUtil.configDataFullPath);
			}
		}

		[MenuItem("GameFrameWork/Start Up", false, 0)]
		public static void GameFrameWorkStartUp()
		{
			if (UnityEditor.EditorUtility.DisplayDialog("提示", "是否以当前场景作为框架启动场景？", "确认", "取消"))
			{
				CreateEntry();
			}
		}

		static bool isCreateEntry = false;
		private static void CreateEntry()
		{
			PlayerPrefs.SetInt("create_entry_script", 1);
            string[] entryScript = EditorUtil.GetAssemblyTypeNames("GameFrameWork.GameFrameWorkEntry", true, "GameFrameWorkEntry");

            if (entryScript == null || entryScript.Length < 1)
            {
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("using GameFrameWork;");
                sb.AppendLine("using UnityEngine;");
                sb.AppendLine();
                sb.AppendLine("public class GameEntry : GameFrameWorkEntry");
                sb.AppendLine("{");
                sb.AppendLine("\tprotected override void OnInit(GameObject manager)");
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine();
                sb.AppendLine("\tprotected override void OnStartGame()");
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine();
                sb.AppendLine("\tprotected override void OnExit()");
                sb.AppendLine("\t{");
                sb.AppendLine();
                sb.AppendLine("\t}");
                sb.AppendLine();
                sb.Append("}");

				Utilities.FileUtil.VerifyDirectory(Application.dataPath + "/Scripts/");
				File.WriteAllText(Application.dataPath + "/Scripts/GameEntry.cs", sb.ToString());
				AssetDatabase.Refresh();
            }
			else
			{
				OnScriptReload();
			}
        }

		[UnityEditor.Callbacks.DidReloadScripts(0)]
		private static void OnScriptReload()
		{
			if(PlayerPrefs.GetInt("create_entry_script",0) == 0)
			{
				return;
			}

            Type[] entryTypes = EditorUtil.GetAssemblyTypes("GameFrameWork.GameFrameWorkEntry", "GameFrameWorkEntry");

			if (entryTypes == null || entryTypes.Length < 1)
			{
				return;
			}

            GameObject go = GameObject.Find("GameEntry");

            if (go == null)
            {
                go = new GameObject("GameEntry");
            }

            if (go.GetComponent(entryTypes[0]) == null)
            {
                go.AddComponent(entryTypes[0]);
            }

            PlayerPrefs.SetInt("create_entry_script", 0);
        }

        [MenuItem("GameFrameWork/UI/创建UI场景")]
		public static void NewUIScene()
		{
			UIEditorInit.NewUIScene();
		}

		[MenuItem("GameFrameWork/SpriteSpliter")]
		public static void OpenSpriteSpliter()
		{
			Rect wr = new Rect(0, 0, 600, 300);
			EditorWindow window = EditorWindow.GetWindowWithRect(typeof(SpriteSplitWindow), wr);
			window.Show();
		}

		[MenuItem("GameFrameWork/AssetBundleEditor")]
		public static void AssetBundleEditor()
		{
			Rect wr = new Rect(0, 0, 700, 800);
			EditorWindow window = EditorWindow.GetWindowWithRect(typeof(AssetBundleWindow), wr);
			window.Show();
		}

		[MenuItem("GameFrameWork/BehaviourTreeEditor")]
		public static void BehaviourTreeEditor()
		{
			EditorWindow window = EditorWindow.GetWindow<BehaviourTreeWindow>();
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/Tab")]
		public static void TabDemoWinow()
		{
			Rect wr = new Rect(0, 0, 600, 600);
			TabDemo window = EditorWindow.GetWindowWithRect<TabDemo>(wr, true, "Unity Tab表签");
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/Styles&Icons")]
		public static void BuiltInDemo()
		{
			BuiltInDemo window = EditorWindow.GetWindow<BuiltInDemo>();
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/SplitView")]
		public static void Init()
		{
			EditorWindow window = EditorWindow.GetWindow<SplitViewDemo>();
			window.Show();
		}

		[MenuItem("Assets/GameFrameWork/CreateFont", false, 0)]
		public static void CreateFont()
		{
			FontMaker.CreateMyFontSprite();
		}

		[MenuItem("Assets/GameFrameWork/CreateSpriteAtlas", false, 1)]
		public static void CreateSpriteAtlas()
		{
			SpriteAtlasPacker window = EditorWindow.GetWindow<SpriteAtlasPacker>();
			window.Show();
		}

		[MenuItem("Assets/GameFrameWork/Add scene to Building Setting", false, 2)]
		public static void AddScene()
		{
			if (Selection.objects.Length > 0)
			{
				List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>();
				sceneList.AddRange(EditorBuildSettings.scenes);

				for (int i = 0; i < Selection.objects.Length; i++)
				{
					string assetPath = AssetDatabase.GetAssetPath(Selection.objects[i]);
					if (!Path.GetExtension(assetPath).Equals(".unity")) continue;
					EditorBuildSettingsScene editorBuildSettings = new EditorBuildSettingsScene(assetPath, true);
					sceneList.Add(editorBuildSettings);
				}

				EditorBuildSettings.scenes = sceneList.ToArray();
				AssetDatabase.Refresh();
			}
		}
	}
}