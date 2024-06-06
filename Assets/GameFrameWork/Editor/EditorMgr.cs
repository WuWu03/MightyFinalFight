using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
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

			PlayerPrefs.SetString("entry_scene", EditorSceneManager.GetActiveScene().path);
            PlayerPrefs.SetInt("create_entry_script", 0);
        }

		[MenuItem("GameFrameWork/UI/创建UI场景", false, 101)]
		public static void NewUIScene()
		{
			string entryScene = PlayerPrefs.GetString("entry_scene",string.Empty);

			if (string.IsNullOrEmpty(entryScene))
			{
                UnityEditor.EditorUtility.DisplayDialog("提示", "未设置[框架启动场景]\n通过[GameFrameWord/StartUp]选项设置", "确定");
                return;
			}

            if (EditorSceneManager.GetActiveScene().path != entryScene)
            {
                EditorSceneManager.OpenScene(entryScene);
            }

			if (string.IsNullOrEmpty(AppConfig.instance.uiDirectory))
			{
                UnityEditor.EditorUtility.DisplayDialog("提示", "未设置UI路径\n在[框架启动场景]选中[GameEntry]并设置[uiDirectory]字段", "确定");
                return;
            }

			UIEditorInit.NewUIScene();
		}


        [MenuItem("GameFrameWork/UI/UI列表", false, 102)]
        public static void OpenUIListWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<UIListWindow>();
            window.Show();
        }

        [MenuItem("GameFrameWork/Tools/切图工具", false, 103)]
		public static void OpenSpriteSpliterTool()
		{
			Rect rect = new Rect(0, 0, 600, 300);
			EditorWindow window = EditorWindow.GetWindowWithRect<SpriteSplitTool>(rect);
			window.Show();
		}

		[MenuItem("GameFrameWork/Tools/PlayerPrefs工具", false, 104)]
		public static void OpenPlayerPrefsTool()
		{
			Rect rect = new Rect(0, 0, 600, 300);
			EditorWindow window = EditorWindow.GetWindowWithRect<PlayerPrefsTool>(rect);
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

		[MenuItem("GameFrameWork/EditorDemo/Tab", false, 1001)]
		public static void TabDemoWinow()
		{
			Rect wr = new Rect(0, 0, 600, 600);
			TabDemo window = EditorWindow.GetWindowWithRect<TabDemo>(wr, true, "Unity Tab表签");
			window.Show();
		}

		[MenuItem("GameFrameWork/EditorDemo/Styles&Icons", false, 1002)]
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

		[MenuItem("Assets/GameFrameWork/CreateUISpriteAtlas", false, 1)]
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

					if (!Path.GetExtension(assetPath).Equals(".unity")) 
					{
						continue; 
					}

					EditorBuildSettingsScene editorBuildSettings = new EditorBuildSettingsScene(assetPath, true);
					sceneList.Add(editorBuildSettings);
				}

				if (sceneList.Count > 0)
				{
					EditorBuildSettings.scenes = sceneList.ToArray();
					AssetDatabase.Refresh();
				}
			}
		}

        [MenuItem("Assets/GameFrameWork/CopyPath", false, 3)]
        private static void CopyAssetsPath()
        {
            if (Selection.activeObject == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "没有选中任何物体", "确定");
                return;
            }

            EditorUtil.CopyTextEditor(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem("GameObject/CopyPath", false, 0)]
        private static void CopyGameObjectPath()
        {
            if (Selection.activeGameObject == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "没有选中任何物体", "确定");
                return;
            }

            StringBuilder sb = new StringBuilder();

			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				string nodePath = EditorUtil.GetNodePath(Selection.gameObjects[i].transform);

				sb.Append("\"");
				sb.Append(nodePath);
				sb.Append("\"");

				if (i < Selection.gameObjects.Length - 1)
				{
					sb.Append("\n");
				}
			}

            EditorUtil.CopyTextEditor(sb.ToString());
        }
    }
}