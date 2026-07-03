#if UNITY_EDITOR
using WuWuFramework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WuWuFramework.Event;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Resources
{
    public static class EditorResourcesMgr
    {
        private static readonly Dictionary<string, UnityObject> s_LoadedAssets = new();
        private static readonly Dictionary<string, List<LoadRequest>> s_LoadRequests = new();
        
        public static UnityObject LoadSync(string assetPath, Type t = null)
        {
            Log.LogInfo("开始加载编辑器资源 : [<color=#FFFF00>", assetPath, "</color>]");
            return OnLoadAssetSync(assetPath, t);
        }

        public static void LoadAsync(string assetPath, Type assetType, WuWuFrameworkAction<string, UnityObject, object> loadedAction, object arg = null)
        {
            LoadRequest loadRequest = LoadRequest.Create(assetPath, assetType, loadedAction, arg);

            if (!s_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> requests))
            {
                requests = new List<LoadRequest> { loadRequest };
                s_LoadRequests.Add(assetPath, requests);
                MonoBehaviourMgr.instance.StartCoroutine(OnLoadAssetAsync(assetPath, assetType));
            }
            else
            {
                requests.Add(loadRequest);
            }
        }

        public static void UnLoad(string assetPath)
        {
            Log.LogInfo("开始卸载编辑器资源 : [<color=#FF0000>", assetPath, "</color>] , ", "卸载前资源数为 : ", s_LoadedAssets.Count.ToString());

            if (s_LoadedAssets.ContainsKey(assetPath))
            {
                s_LoadedAssets.Remove(assetPath);
            }

            Log.LogInfo("卸载编辑器资源 : [<color=#FF0000>", assetPath, "</color>] 完成 , ", "卸载后资源数为 : ", s_LoadedAssets.Count.ToString());
        }

        public static void UnLoadAll()
        {
            List<string> assetNames = s_LoadedAssets.Keys.ToList();

            foreach (var assetName in assetNames)
            {
                UnLoad(assetName);
            }

            s_LoadedAssets.Clear();
            s_LoadRequests.Clear();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        private static UnityObject OnLoadAssetSync(string assetPath, Type t)
        {
            if (s_LoadedAssets.TryGetValue(assetPath, out UnityEngine.Object obj))
            {
                return obj;
            }

            string filePath = PathUtil.GetAssetPath(assetPath);
            obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePath, t);

            if (obj == null)
            {
                Log.LogError("加载失败 , 资源路径不存在 : [<color=#FF0000>", assetPath, "</color>]");
                return null;
            }

            s_LoadedAssets.Add(assetPath, obj);
            return obj;
        }

        /// <summary>
        /// 模拟异步加载
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="t"></param>
        private static IEnumerator OnLoadAssetAsync(string assetPath, Type t = null)
        {
            yield return null;
            UnityObject obj = LoadSync(assetPath, t);
            yield return null;
            
            if (obj is null)
            {
                throw new Exception(StringUtil.Append("加载 [", assetPath,"] 失败"));
            }
            
            if (!s_LoadRequests.Remove(assetPath, out List<LoadRequest> loadRequests))
            {
                throw new Exception(StringUtil.Append("加载 [", assetPath,"] 成功，但回调不存在"));
            }
            
            foreach (var loadRequest in loadRequests)
            {
                if (loadRequest.loadedAction != null)
                {
                    loadRequest.Loaded(obj);
                }

                loadRequest.Release();
            }
        }
    }
}
#endif