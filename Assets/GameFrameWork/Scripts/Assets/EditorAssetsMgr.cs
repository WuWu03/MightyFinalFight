#if UNITY_EDITOR
using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFrameWork.Assets
{
    public static class EditorAssetsMgr
    {
        public static UnityEngine.Object LoadAssetSync(string assetPath, Type t = null)
        {
            Log.LogInfo("开始加载编辑器资源 : [<color=#FFFF00>", assetPath, "</color>]");
            return OnLoadAssetSync(assetPath, t);
        }

        public static void LoadAssetAsync(MonoBehaviour mono, string assetPath, Type assetType, GameFrameWorkAction<string, UnityEngine.Object, object> loadedAction, object arg = null)
        {
            LoadRequest loadRequest = LoadRequest.Create(assetPath, assetType, loadedAction, arg);

            if (!m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> requests))
            {
                requests = new List<LoadRequest>() { loadRequest };
                m_LoadRequests.Add(assetPath, requests);
                mono.StartCoroutine(OnLoadAssetAsync(assetPath, assetType));
            }
            else
            {
                requests.Add(loadRequest);
            }
        }

        public static void UnLoadAssetEditor(string assetPath)
        {
            Log.LogInfo("开始卸载编辑器资源 : [<color=#FF0000>", assetPath, "</color>] , ", "卸载前资源数为 : ", m_LoadedAssets.Count.ToString());

            if (m_LoadedAssets.ContainsKey(assetPath))
            {
                m_LoadedAssets.Remove(assetPath);
            }

            Log.LogInfo("卸载编辑器资源 : [<color=#FF0000>", assetPath, "</color>] 完成 , ", "卸载后资源数为 : ", m_LoadedAssets.Count.ToString());
        }

        public static void UnLoadAll()
        {
            List<string> list = m_LoadedAssets.Keys.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                UnLoadAssetEditor(list[i]);
            }

            m_LoadedAssets.Clear();
            m_LoadRequests.Clear();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        private static UnityEngine.Object OnLoadAssetSync(string assetPath, Type t)
        {
            if (m_LoadedAssets.TryGetValue(assetPath, out UnityEngine.Object obj))
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

            m_LoadedAssets.Add(assetPath, obj);
            return obj;
        }

        // 模拟异步加载的行为
        private static IEnumerator OnLoadAssetAsync(string assetPath, Type t = null)
        {
            yield return null;
            UnityEngine.Object obj = LoadAssetSync(assetPath, t);
            yield return null;

            if (m_LoadRequests.TryGetValue(assetPath, out List<LoadRequest> list))
            {
                if (obj != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].loadedAction != null)
                        {
                            list[i].Loaded(obj);
                        }

                        list[i].Release();
                    }
                }

                m_LoadRequests.Remove(assetPath);
            }
        }

        private static readonly Dictionary<string, UnityEngine.Object> m_LoadedAssets = new();
        private static readonly Dictionary<string, List<LoadRequest>> m_LoadRequests = new();
    }
}
#endif