using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameFrameWork.Editor
{
    [InitializeOnLoad]
    public static class EditorMgr
	{
        public static GameFrameWorkConfigWindowData GetGameFrameWorkConfig()
        {
            GameFrameWorkConfigWindowData config = AssetDatabase.LoadAssetAtPath<GameFrameWorkConfigWindowData>(EditorPathUtil.gameFrameWorkConfigWindowDataPath);
            return config;
        }

        [MenuItem("GameFrameWork/Start Up &1", false,0)]
		public static void GameFrameWorkStartUp()
		{
            CreateEntryScript();

            Rect rect = new Rect(0, 0, 600, 300);
            EditorWindow window = EditorWindow.GetWindowWithRect<GameFrameWorkConfigWindow>(rect);
            window.Show();
        }

        /// <summary>
        /// 创建框架启动脚本
        /// </summary>
        private static void CreateEntryScript()
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

                Utils.FileUtil.VerifyDirectory(EditorPathUtil.editorScriptFullPath);
                File.WriteAllText(EditorPathUtil.entryScriptFullPath, sb.ToString());
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 主要用于第一次启动Unity编辑器和编译完成后，检查是否已经设置框架启动场景
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts(0)]
		private static void OnScriptReload()
		{
            EditorApplication.update += CheckIsInit;
		}

        private static bool m_HasAddSceneChangeEvent = false;
        private static void CheckIsInit()
		{
			if (string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
			{
				return;
			}

            if(!m_HasAddSceneChangeEvent)
            {
                m_HasAddSceneChangeEvent = true;
                EditorSceneManager.sceneOpened += (scene, mode) =>
                {
                    CheckEntryScene();
                };
            }

            EditorApplication.update -= CheckIsInit;
            CheckEntryScene();
		}

        private static bool m_IsShowMainScene = false;
        private static void CheckEntryScene()
        {
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                m_IsShowMainScene = true;
                GameFrameWorkStartUp();
                return;
            }

            if(!m_IsShowMainScene)
            {
                m_IsShowMainScene = true;
                GoToGameFrameWorkEntryScene();
            }
        }

        /// <summary>
        /// 跳转到框架启动场景
        /// </summary>
        public static void GoToGameFrameWorkEntryScene()
		{
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                return;
			}

			if (!EditorSceneManager.GetActiveScene().path.Equals(config.entryScene))
			{
                EditorSceneManager.OpenScene(config.entryScene);
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
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                bool result = UnityEditor.EditorUtility.DisplayDialog("提示", "未设置UI目录，点击确定前往设置", "确定");

                if(result)
                {
                    GameFrameWorkStartUp();
                }

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

        [MenuItem("GameFrameWork/AssetBundleEditor",false,105)]
		public static void AssetBundleEditor()
		{
			Rect wr = new Rect(0, 0, 700, 800);
			EditorWindow window = EditorWindow.GetWindowWithRect(typeof(AssetBundleWindow), wr);
			window.Show();
		}

		[MenuItem("GameFrameWork/BehaviourTreeEditor",false , 106)]
		public static void BehaviourTreeEditor()
		{
			EditorWindow window = EditorWindow.GetWindow<BehaviourTreeWindow>();
			window.Show();
		}

        [MenuItem("GameFrameWork/Build/Build Game",false , 107)]
        public static void BuildGame()
        {
            BuildGame(false);

        }

        [MenuItem("GameFrameWork/Build/Build Game Log",false , 108)]
        public static void BuildGameLog()
        {
            BuildGame(true);
        }

        private static void BuildGame(bool openLog)
        {
            GameFrameWorkConfigWindowData config = GetGameFrameWorkConfig();
            if (config == null || string.IsNullOrEmpty(config.entryScene))
            {
                if (UnityEditor.EditorUtility.DisplayDialog("提示", "没有设置框架启动场景，点击确认前往设置", "确认"))
                {
                    GameFrameWorkStartUp();
                }

                return;
            }

            if (!UnityEditor.EditorUtility.DisplayDialog("提示", "点击确认开始打包", "确认", "取消"))
            {
                return;
            }

            bool isLoadFromAssetBundle = config.isLoadFromAssetBundle;
            bool isOpenLog = config.isOpenLog;

            config.isLoadFromAssetBundle = true;
            config.isOpenLog = openLog;

            EditorSceneManager.SaveOpenScenes();

            using (AssetBundleBuilder builder = new AssetBundleBuilder())
            {
                builder.Build(BuildTarget.StandaloneWindows, false);
            }

            AssetDatabase.Refresh();

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
#if UNITY_ANDROID
            string buildPath = config.androidBuildPath;
            string ext = ".apk";
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.target = BuildTarget.Android;
#elif UNITY_IOS
            string buildPath = config.iosBuildPath;
            string ext = ".ipa";
            buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;
            buildPlayerOptions.target = BuildTarget.iOS;
#else
            string buildPath = config.pcBuildPath;
            string ext = ".exe";
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
#endif
            if (string.IsNullOrEmpty(buildPath))
            {
                buildPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + Application.productName + "\\" + Application.productName + ext;
            }

            string[] scenes = new string[EditorBuildSettings.scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.options = BuildOptions.None;
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary buildSummary = buildReport.summary;

            if (buildSummary.result == BuildResult.Succeeded)
            {
                config = GetGameFrameWorkConfig();
                config.isLoadFromAssetBundle = isLoadFromAssetBundle;
                config.isOpenLog = isOpenLog;
                EditorSceneManager.SaveOpenScenes();
                UnityEditor.EditorUtility.DisplayDialog("提示", "打包成功", "确认");
                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(buildPath) + @"\");
            }
            else if (buildSummary.result == BuildResult.Failed)
            {
                config = GetGameFrameWorkConfig();
                config.isLoadFromAssetBundle = isLoadFromAssetBundle;
                config.isOpenLog = isOpenLog;
                Debug.LogError("Build windows error : [" + buildSummary.ToString() + "]");
            }
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


        [MenuItem("Assets/GameFrameWork/SetLanguageKeyFile", false, 4)]
        private static void SetLanguageKeyFile()
        {
            if (Selection.activeObject == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "没有选中任何物体", "确定");
                return;
            }

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if(Path.GetExtension(path) != ".txt")
            {
                UnityEditor.EditorUtility.DisplayDialog("提示", "请选中一个文本文件", "确定");
                return;
            }

            PlayerPrefs.SetString("unity_editor_language_key_file", path);
            UnityEditor.EditorUtility.DisplayDialog("提示", "设置多语言检测文件成功", "确定");
            AssetDatabase.Refresh();
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