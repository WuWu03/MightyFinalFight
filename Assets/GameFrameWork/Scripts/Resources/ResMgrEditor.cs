using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using GameFrameWork.Utilities;
using GameFrameWork.Log;

namespace GameFrameWork.Resources
{
    public class ResMgrEditor:Singleton<ResMgrEditor>
    {
#if UNITY_EDITOR
        class LoadRequest 
        {
            public GameFrameWorkAction<string, UnityEngine.Object, object[]> onLoadEvent;
            public object[] args;
        }

        public ResMgrEditor()
        {
            m_LoadedAssets = new Dictionary<string, UnityEngine.Object>();
            m_DicLoadRequest = new Dictionary<string, List<LoadRequest>>();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resourcePath">资源路径</param>
        /// <returns>资源对象</returns>
        private UnityEngine.Object Load(string resourcePath, Type t)
        {
            UnityEngine.Object obj;

            if (m_LoadedAssets.TryGetValue(resourcePath, out obj))
            {
                return obj;
            }

            string fileName = Path.GetFileName(resourcePath);
            string dir = PathUtil.FormatPath("Assets", Path.GetDirectoryName(resourcePath));
            string paName = StringUtil.FormatDefault(fileName, "*");
            string[] files = Directory.GetFiles(dir, paName, SearchOption.TopDirectoryOnly);

            // 加载本地资源
            for (int i = 0, UPPER = files.Length; i < UPPER; i++)
            {
                if (Path.GetExtension(files[i]) == ".meta")
                {
                    continue;
                }

                GameFrameworkLog.Log(StringUtil.FormatDefault("开始编辑器加载资源：", files[i]));
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(files[i], t);
                break;
            }

            if (obj == null)
            {
                GameFrameworkLog.Log(StringUtil.FormatDefault("无效的资源路径 => ", resourcePath));
                return null;
            }

            m_LoadedAssets.Add(resourcePath, obj);
            return obj;
        }

        public void LoadForEditorAsync(string resourcePath, GameFrameWorkAction<string, UnityEngine.Object, object[]> action = null, Type t = null, object[] param = null)
        {
            List<LoadRequest> list = null;

            if (!m_DicLoadRequest.TryGetValue(resourcePath, out list))
            {
                list = new List<LoadRequest>();
                m_DicLoadRequest.Add(resourcePath, list);
            }

            list.Add(new LoadRequest() { onLoadEvent = action, args = param });
            ResMgr.instance.StartCoroutine(InnerLoad(resourcePath, t));
        }

        public UnityEngine.Object LoadForEditor(string resourcePath, Type t = null)
        {
            return Load(resourcePath, t);
        }

        // 模拟异步加载的行为
        private IEnumerator InnerLoad(string resourcePath, Type t = null)
        {
            // 等待一帧
            UnityEngine.Object obj = Load(resourcePath, t);
            // 等待一帧
            yield return null;
            //// 等待一秒
            //yield return new WaitForSeconds(0.1f);
            // 返回资源
            List<LoadRequest> list = null;

            if (m_DicLoadRequest.TryGetValue(resourcePath, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].onLoadEvent?.Invoke(resourcePath, obj, list[i].args);
                }

                m_DicLoadRequest.Remove(resourcePath);
            }
        }

        private Dictionary<string, UnityEngine.Object> m_LoadedAssets = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequest = null;
#endif
    }
}