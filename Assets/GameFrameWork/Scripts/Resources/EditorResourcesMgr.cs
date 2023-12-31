using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GameFrameWork.Resources
{
    public class EditorResourcesMgr : Singleton<EditorResourcesMgr>
    {
#if UNITY_EDITOR
        //class LoadRequest 
        //{
        //    public GameFrameWorkAction<string, UnityEngine.Object, object[]> onLoadEvent;
        //    public object[] args;
        //}

        public EditorResourcesMgr()
        {
            m_LoadedAssets = new Dictionary<string, UnityEngine.Object>();
            m_DicLoadRequest = new Dictionary<string, List<LoadRequest>>();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns>资源对象</returns>
        private UnityEngine.Object Load(string assetPath, Type t)
        {
            UnityEngine.Object obj;

            if (m_LoadedAssets.TryGetValue(assetPath, out obj))
            {
                return obj;
            }

            string fileName = Path.GetFileName(assetPath);
            string dir = PathUtil.FormatPath("Assets", Path.GetDirectoryName(assetPath));
            string paName = StringUtil.Format(fileName, "*");
            string[] files = Directory.GetFiles(dir, paName, SearchOption.TopDirectoryOnly);

            // 加载本地资源
            for (int i = 0, UPPER = files.Length; i < UPPER; i++)
            {
                if (Path.GetExtension(files[i]) == ".meta")
                {
                    continue;
                }

                Log.LogInfo(StringUtil.Format("开始编辑器加载资源：", files[i]));
                obj = UnityEditor.AssetDatabase.LoadAssetAtPath(files[i], t);
                break;
            }

            if (obj == null)
            {
                Log.LogInfo(StringUtil.Format("无效的资源路径 => ", assetPath));
                return null;
            }

            m_LoadedAssets.Add(assetPath, obj);
            return obj;
        }

        public void LoadForEditorAsync(string assetPath, GameFrameWorkAction<string, UnityEngine.Object, object[]> action = null, Type t = null, object[] param = null)
        {
            List<LoadRequest> list = null;

            if (!m_DicLoadRequest.TryGetValue(assetPath, out list))
            {
                list = new List<LoadRequest>();
                m_DicLoadRequest.Add(assetPath, list);
            }

            list.Add(new LoadRequest(assetPath, action, param));
            ResourcesMgr.instance.StartCoroutine(InnerLoad(assetPath, t));
        }

        public UnityEngine.Object LoadForEditor(string assetPath, Type t = null)
        {
            return Load(assetPath, t);
        }

        // 模拟异步加载的行为
        private IEnumerator InnerLoad(string assetPath, Type t = null)
        {
            UnityEngine.Object obj = Load(assetPath, t);
            // 等待一帧
            yield return null;
            //// 等待一秒
            //yield return new WaitForSeconds(0.1f);
            // 返回资源
            List<LoadRequest> list = null;

            if (m_DicLoadRequest.TryGetValue(assetPath, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Call(obj);
                }

                m_DicLoadRequest.Remove(assetPath);
            }
        }

        private Dictionary<string, UnityEngine.Object> m_LoadedAssets = null;
        private Dictionary<string, List<LoadRequest>> m_DicLoadRequest = null;
#endif
    }
}