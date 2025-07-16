using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public static class BuildTool
    {
        struct BuildConfig
        {
            public string ext;
            public BuildTargetGroup targetGroup;
            public BuildTarget target;
        }

        public static void Build(BuildTarget buildTarget,string buildPath)
        {
            using (AssetBundleBuilder builder = new AssetBundleBuilder())
            {
                if (!builder.Build(buildTarget, false))
                {
                    return;
                }
            }

            BuildConfig buildConfig = m_DicBuildConfig.GetValueOrDefault(buildTarget);
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions() 
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
        }

        private static Dictionary<BuildTarget, BuildConfig> m_DicBuildConfig = new Dictionary<BuildTarget, BuildConfig>()
        {
            { 
                BuildTarget.Android , new BuildConfig 
                { 
                    ext = ".apk" , 
                    targetGroup = BuildTargetGroup.Android, 
                    target = BuildTarget.Android,
                } 
            },

            {
                BuildTarget.iOS , new BuildConfig
                {
                    ext = ".ipa",
                    targetGroup = BuildTargetGroup.iOS,
                    target = BuildTarget.iOS,
                }
            },

            {
                BuildTarget.StandaloneWindows64 , new BuildConfig
                {
                    ext = ".exe" ,
                    targetGroup = BuildTargetGroup.Standalone,
                    target = BuildTarget.StandaloneWindows64,
                }
            },

            {
                BuildTarget.StandaloneWindows , new BuildConfig
                {
                    ext = ".exe" ,
                    targetGroup = BuildTargetGroup.Standalone,
                    target = BuildTarget.StandaloneWindows
                }
            },
        };
    }
}