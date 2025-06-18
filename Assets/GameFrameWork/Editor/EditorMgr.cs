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

        [MenuItem("GameFrameWork/Start Up &1", false,0)]
		public static void GameFrameWorkStartUp()
		{
            string entryScenePath = PlayerPrefs.GetString("unity_editor_entry_scene", string.Empty);

			if (string.IsNullOrEmpty(entryScenePath))
			{
				CheckEntryScene();
                return;
            }

            if (EditorSceneManager.GetActiveScene().path.Equals(entryScenePath))
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "当前已经位于启动场景", "确认");
				return;
            }

            bool result = UnityEditor.EditorUtility.DisplayDialog("提示", "已创建过启动场景，是否跳转？", "确认", "取消");

            if (result)
            {
                CheckEntryScene();
            }
        }

		/// <summary>
		/// 检查框架启动场景，若未创建则弹提示框进行创建，若已创建则跳转到启动场景
		/// </summary>
        public static bool CheckEntryScene()
        {
            string entryScenePath = PlayerPrefs.GetString("unity_editor_entry_scene", string.Empty);

            if (string.IsNullOrEmpty(entryScenePath))
            {
                Type[] entryTypes = EditorUtil.GetAssemblyTypes("GameFrameWork.GameFrameWorkEntry", "GameFrameWorkEntry");

                if (entryTypes == null || entryTypes.Length < 1)
                {
                    bool result = UnityEditor.EditorUtility.DisplayDialog("提示", "是否以当前场景作为框架启动场景？", "确认", "取消");

					if(result)
					{
                        CreateEntry();
                    }

					return result;
                }
                else
                {
					bool result = UnityEditor.EditorUtility.DisplayDialog("提示", "尚未设置框架启动场景，是否以当前场景作为框架启动场景？", "确认", "取消");
                    if (result)
                    {
                        PlayerPrefs.SetString("unity_editor_entry_scene", EditorSceneManager.GetActiveScene().path);
                        GoToGameFrameWorkEntryScene();
                    }
                    return result;
                }
            }

            GoToGameFrameWorkEntryScene();
            return true;
        }

        /// <summary>
        /// 创建框架启动脚本
        /// </summary>
        private static void CreateEntry()
		{
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

            PlayerPrefs.SetString("unity_editor_entry_scene", EditorSceneManager.GetActiveScene().path);
            GoToGameFrameWorkEntryScene();
        }

        /// <summary>
        /// 主要用于第一次启动Unity编辑器时，检查是否已经初始化框架启动场景
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts(0)]
		private static void OnScriptReload()
		{
			int isInit = PlayerPrefs.GetInt("unity_editor_is_init", 0);

            if (isInit == 1)
			{
                EditorApplication.quitting += () =>
                {
                    PlayerPrefs.SetInt("unity_editor_is_init", 0);
                };

                EditorApplication.update -= CheckIsInit;
				return;
			}

			EditorApplication.update += CheckIsInit;
		}

		private static void CheckIsInit()
		{
			if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
			{
				return;
			}

            PlayerPrefs.SetInt("unity_editor_is_init", 1);
            EditorApplication.update -= CheckIsInit;
			CheckEntryScene();
		}

        /// <summary>
        /// 跳转到框架启动场景
        /// </summary>
        private static void GoToGameFrameWorkEntryScene()
		{
            string entryScenePath = PlayerPrefs.GetString("unity_editor_entry_scene", string.Empty);

            if (string.IsNullOrEmpty(entryScenePath))
			{
				return;
			}

			if (!EditorSceneManager.GetActiveScene().path.Equals(entryScenePath))
			{
                EditorSceneManager.OpenScene(entryScenePath);
			}

            Type[] entryTypes = EditorUtil.GetAssemblyTypes("GameFrameWork.GameFrameWorkEntry", "GameFrameWorkEntry");

            if (entryTypes == null || entryTypes.Length < 1)
            {
                return;
            }

			GameObject entry = GameObject.Find("GameEntry");

            if (entry == null)
            {
                entry = new GameObject("GameEntry");
                entry.AddComponent(entryTypes[0]);
            }
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