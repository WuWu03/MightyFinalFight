using GameFrameWork;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public class Utility
    {
        public static void CreateConfigData<T, P>(string name, string ext, string dir = null)
                    where T : BaseScriptableObject<P>
                    where P : BaseConfigData
        {
            string directory = Application.dataPath + "/ConfigData/";
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

            T data = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(data, directory.Substring(directory.IndexOf("Assets")) + name + ext);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}