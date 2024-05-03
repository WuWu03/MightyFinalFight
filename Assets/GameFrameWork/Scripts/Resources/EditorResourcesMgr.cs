#if UNITY_EDITOR
using GameFrameWork.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameFrameWork.Resources
{
    public class EditorResourcesMgr : Singleton<EditorResourcesMgr>
    {
        public EditorResourcesMgr()
        {
            m_DicLoadedAssets = new Dictionary<string, UnityEngine.Object>();
            m_DicLoadRequests = new Dictionary<string, List<LoadRequest>>();
        }

        public UnityEngine.Object LoadAssetSync(string assetPath, Type t = null)
        {
            Log.LogInfo("开始加载编辑器资源 : [<color=#FFFF00>", assetPath, "</color>]");
            return OnLoadAssetSync(assetPath, t);
        }

        public void LoadAssetAsync(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> action = null, Type t = null, params object[] args)
        {
            LoadRequest loadRequest = LoadRequest.Create();
            loadRequest.assetPath = assetPath;
            loadRequest.action = action;
            loadRequest.args = args;


            if (!m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> requests))
            {
                requests = new List<LoadRequest>() { loadRequest };
                m_DicLoadRequests.Add(assetPath, requests);
                ResourcesMgr.instance.StartCoroutine(OnLoadAssetAsync(assetPath, t));
            }
            else
            {
                requests.Add(loadRequest);
            }
        }

        public void UnLoadAssetEditor(string assetPath)
        {
            Log.LogInfo("开始卸载编辑器资源 : [<color=#FF0000>", assetPath, "</color>] , ", "卸载前资源数为 : ",m_DicLoadedAssets.Count);

            if (m_DicLoadedAssets.ContainsKey(assetPath))
            {
                m_DicLoadedAssets.Remove(assetPath);
            }

            Log.LogInfo("卸载编辑器资源 : [<color=#FF0000>", assetPath, "</color>] 完成 , ", "卸载后资源数为 : ", m_DicLoadedAssets.Count);
        }

        public void UnLoadAll()
        {
            List<string> list = m_DicLoadedAssets.Keys.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                UnLoadAssetEditor(list[i]);
            }

            m_DicLoadedAssets.Clear();
            m_DicLoadedAssets.Clear();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        private UnityEngine.Object OnLoadAssetSync(string assetPath, Type t)
        {
            if (m_DicLoadedAssets.TryGetValue(assetPath, out UnityEngine.Object obj))
            {
                return obj;
            }

            string filePath = PathUtil.GetAssetPath(assetPath);
            string fileName = Path.GetFileName(assetPath);
            string directoryName = Path.GetDirectoryName(filePath).Replace("\\", "/");
            string searchParttern = StringUtil.Format(fileName, "*");
            string[] files = FileUtil.GetFiles(directoryName, searchParttern);

            obj = UnityEditor.AssetDatabase.LoadAssetAtPath(files[0], t);

            if (obj == null)
            {
                Log.LogInfo("加载失败 , 资源路径不存在 : ", assetPath);
                return null;
            }

            m_DicLoadedAssets.Add(assetPath, obj);
            return obj;
        }

        // 模拟异步加载的行为
        private IEnumerator OnLoadAssetAsync(string assetPath, Type t = null)
        {
            yield return null;
            UnityEngine.Object obj = LoadAssetSync(assetPath, t);
            yield return null;

            if (m_DicLoadRequests.TryGetValue(assetPath, out List<LoadRequest> list))
            {
                if (obj != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].action != null)
                        {
                            list[i].Call(obj);
                        }

                        ReferencePool.ReleaseReference(list[i]);
                    }
                }

                m_DicLoadRequests.Remove(assetPath);
            }
        }

        private Dictionary<string, UnityEngine.Object> m_DicLoadedAssets = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequests = null;
    }
}
#endif