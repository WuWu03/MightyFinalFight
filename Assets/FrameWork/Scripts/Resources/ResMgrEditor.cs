using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.IO;

namespace FrameWork.Resources
{
    public class ResMgrEditor
    {

#if UNITY_EDITOR
        private static ResMgrEditor _ins;

        public static ResMgrEditor Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ResMgrEditor();
                }
                return _ins;
            }

            set
            {
                _ins = value;
            }
        }
        private Dictionary<string, Object> m_LoadedAssets = new Dictionary<string, Object>();
        private Dictionary<string, List<Action<Object>>> m_DicLoadRequest = new Dictionary<string, List<Action<Object>>>();
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resourcePath">资源路径</param>
        /// <returns>资源对象</returns>
        private Object LoadForEditor(string resourcePath, Type t)
        {
            Object obj;
            if (m_LoadedAssets.TryGetValue(resourcePath, out obj))
                return obj;

            string fileName = Path.GetFileName(resourcePath);

            string dir = string.Format("Assets/{0}", Path.GetDirectoryName(resourcePath));
            string paName = string.Format("{0}*", fileName);
            string[] files = Directory.GetFiles(dir, paName, SearchOption.TopDirectoryOnly);

            // 加载本地资源
            for (int i = 0, UPPER = files.Length; i < UPPER; i++)
            {
                if (Path.GetExtension(files[i]) == ".meta") continue;

                Debug.Log("开始编辑器加载资源：" + files[i]);
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(files[i], t);
                break;
            }
            if (obj == null)
            {
                Debug.Log("无效的资源路径 => " + resourcePath);
                return null;
            }
            return obj;
        }

        public void LoadForEditorAsync(string resourcePath, Action<Object> action = null, Type t = null)
        {
            List<Action<Object>> list = null;

            if (!m_DicLoadRequest.TryGetValue(resourcePath, out list))
            {
                list = new List<Action<Object>>();
                m_DicLoadRequest.Add(resourcePath, list);
            }

            list.Add(action);
            ResMgr.Ins.StartCoroutine(InnerLoad(resourcePath, t));
        }

        // 模拟异步加载的行为
        private IEnumerator InnerLoad(string resourcePath, Type t = null)
        {
            // 等待一帧
            Object obj = LoadForEditor(resourcePath, t);
            // 等待一帧
            yield return null;
            //// 等待一秒
            //yield return new WaitForSeconds(0.1f);
            // 返回资源
            List<Action<Object>> list = null;

            if (m_DicLoadRequest.TryGetValue(resourcePath, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i]?.Invoke(obj);
                }

                m_DicLoadRequest.Remove(resourcePath);
            }
        }
#endif
    }
}