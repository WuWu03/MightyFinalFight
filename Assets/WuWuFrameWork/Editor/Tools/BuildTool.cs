using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace WuWuFramework.Editor
{
    public static class BuildTool
    {
        struct BuildConfig
        {
            public string ext;
            public BuildTargetGroup targetGroup;
            public BuildTarget target;
        }

        public static void Build(BuildTarget buildTarget, string buildPath)
        {
            AddBuildTarget(BuildTarget.Android, BuildTargetGroup.Android, ".apk");
            AddBuildTarget(BuildTarget.iOS, BuildTargetGroup.iOS, ".ipa");
            AddBuildTarget(BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone, ".exe");
            AddBuildTarget(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, ".exe");

            using (AssetBundleBuilder builder = new())
            {
                if (!builder.Build(buildTarget, false))
                {
                    return;
                }
            }

            BuildConfig buildConfig = m_BuildConfigs.GetValueOrDefault(buildTarget);
            BuildPlayerOptions buildPlayerOptions = new()
            {
                targetGroup = buildConfig.targetGroup,
                target = buildConfig.target,
            };

            if (string.IsNullOrEmpty(buildPath))
            {
                buildPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            buildPath += "\\" + Application.productName + "\\" + Application.productName + buildConfig.ext;

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
                EditorSceneManager.SaveOpenScenes();
                UnityEditor.EditorUtility.DisplayDialog("提示", "打包成功", "确认");
                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(buildPath) + @"\");
            }
            else if (buildSummary.result == BuildResult.Failed)
            {
                Debug.LogError("Build windows error : [" + buildSummary.ToString() + "]");
            }

            m_BuildConfigs.Clear();
            m_BuildConfigs = null;
        }

        private static void AddBuildTarget(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup, string extend)
        {
            m_BuildConfigs ??= new Dictionary<BuildTarget, BuildConfig>();
            m_BuildConfigs.Add(buildTarget, new BuildConfig
            {
                ext = extend,
                targetGroup = buildTargetGroup,
                target = buildTarget,
            });
        }

        private static Dictionary<BuildTarget, BuildConfig> m_BuildConfigs = null;
    }
}