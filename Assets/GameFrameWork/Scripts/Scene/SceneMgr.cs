using GameFrameWork.Assets;
using GameFrameWork.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFrameWork.Scene
{
    public class SceneMgr : BaseMgr<SceneMgr>
    {
        public event GameFrameWorkAction<LoadSceneSuccessEventArgs> loadSceneSuccessEvent
        {
            add
            {
                m_LoadSceneSuccessEvent += value;
            }
            remove
            {
                m_LoadSceneSuccessEvent -= value;
            }
        }

        public event GameFrameWorkAction<LoadSceneFailureEventArgs> loadSceneFailuerEvent
        {
            add
            {
                m_LoadSceneFailureEvent += value;
            }
            remove
            {
                m_LoadSceneFailureEvent -= value;
            }
        }

        public event GameFrameWorkAction<LoadSceneUpdateEventArgs> loadSceneUpdateEvent
        {
            add
            {
                m_LoadSceneUpdateEvent += value;
            }
            remove
            {
                m_LoadSceneUpdateEvent -= value;
            }
        }

        public event GameFrameWorkAction<UnLoadSceneSuccessEventArgs> unLoadSceneSuccessEvent
        {
            add
            {
                m_UnLoadSceneSuccessEvent += value;
            }
            remove
            {
                m_UnLoadSceneSuccessEvent -= value;
            }
        }

        public event GameFrameWorkAction<UnLoadSceneFailureEventArgs> unLoadSceneFailuerEvent
        {
            add
            {
                m_UnLoadSceneFailureEvent += value;
            }
            remove
            {
                m_UnLoadSceneFailureEvent -= value;
            }
        }

        public bool isLoading
        {
            get
            {
                return m_ListLoadingScene != null && m_ListLoadingScene.Count > 0;
            }
        }

        public bool isUnLoading
        {
            get
            {
                return m_ListUnLoadingScene != null && m_ListUnLoadingScene.Count > 0;
            }
        }

        public string currSceneName
        {
            get
            {
                return m_CurrSceneName;
            }
        }

        public int loadedSceneCount
        {
            get
            {
                return m_ListLoadedScene.Count;
            }
        }

        public int unLoadedSceneCount
        {
            get
            {
                return m_ListUnLoadedScene.Count;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_ListLoadingScene = new List<string>();
            m_ListUnLoadingScene = new List<string>();
            m_ListLoadedScene = new List<string>();
            m_ListUnLoadedScene = new List<string>();
            m_LoadQueue = new Queue<LoadSceneRequest>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (isLoading)
            {
                return;
            }

            if(isUnLoading)
            {
                return;
            }

            if (m_LoadQueue.Count > 0)
            {
                lock (m_LoadQueue)
                {
                    LoadSceneRequest request = m_LoadQueue.Dequeue();

                    if (request.isUnLoad)
                        InnerUnLoadSceneAsync(request);
                    else
                        InnerLoadSceneAsync(request);
                }
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            while (m_LoadQueue.Count > 0)
            {
                ReferencePool.ReleaseReference(m_LoadQueue.Dequeue());
            }

            m_LoadQueue.Clear();
            m_ListLoadingScene.Clear();
            m_ListUnLoadingScene.Clear();
            m_ListLoadedScene.Clear();
            m_ListUnLoadedScene.Clear();
            m_CurrSceneName = string.Empty;
            m_AsyncOperation = null;
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneFailureEvent = null;
            m_LoadSceneUpdateEvent = null;
            m_UnLoadSceneSuccessEvent = null;
            m_UnLoadSceneFailureEvent = null;
        }

        public void LoadSceneAsync(string sceneName)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true);
        }

        public void LoadSceneAsync(string sceneName, params object[] args)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true, args);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene)
        {
            LoadSceneAsync(sceneName, mode, isAutoAllowScene, null);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene, object[] args)
        {
            if (isLoading)
            {
                LoadSceneFailure(sceneName, "加载失败，正在加载中，无法进行加载", null);
                return;
            }

            if (isUnLoading)
            {
                LoadSceneFailure(sceneName, "加载失败，正在卸载中，无法进行加载", null);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Format("加载失败，场景 : [", sceneName, "] 已加载"), null);
                return;
            }

            m_AsyncOperation = null;

            lock (m_LoadQueue)
            {
                m_LoadQueue.Enqueue(LoadSceneRequest.Create(sceneName, args, mode, isAutoAllowScene));
            }
        }

        public void LoadScene(string sceneName)
        {
            LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void LoadScene(string sceneName, params object[] args)
        {
            LoadScene(sceneName, LoadSceneMode.Single, args);
        }

        public void LoadScene(string sceneName, LoadSceneMode mode)
        {
            LoadScene(sceneName, mode, null);
        }

        public void LoadScene(string sceneName, LoadSceneMode mode, object[] args)
        {
            if (isLoading)
            {
                LoadSceneFailure(sceneName, "加载失败，正在加载中，无法进行加载", args);
                return;
            }

            if (isUnLoading)
            {
                LoadSceneFailure(sceneName, "加载失败，正在卸载中，无法进行加载", null);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Format("加载失败，场景 : [", sceneName, "] 已加载"), args);
                return;
            }

            try
            {
                LoadSceneParameters parameters = new LoadSceneParameters() { loadSceneMode = mode };
#if UNITY_EDITOR
                if (!AppConfig.instance.loadAB)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(PathUtil.GetAssetFullPath(sceneName), parameters);
                }
                else
#endif
                {
                    AssetsMgr.instance.LoadAssetSync(sceneName, typeof(UnityEngine.SceneManagement.Scene));
                }
                    SceneManager.LoadScene(sceneName, mode);
                LoadSceneSuccess(sceneName, args);
            }
            catch (Exception e)
            {
                LoadSceneFailure(sceneName, e.Message, args);
            }
        }

        public void UnLoadScene(string sceneName, object[] args)
        {
            if (isLoading)
            {
                UnLoadSceneFailure(sceneName, "卸载失败，正在加载中，无法进行卸载", args);
                return;
            }

            if (isUnLoading)
            {
                UnLoadSceneFailure(sceneName, "卸载失败，正在卸载中，无法进行卸载", null);
                return;
            }

            if (!IsSceneLoaded(sceneName))
            {
                UnLoadSceneFailure(sceneName, StringUtil.Format("卸载失败，场景 : [", sceneName, "] 未加载"), args);
                return;
            }

            lock (m_LoadQueue)
            {
                m_LoadQueue.Enqueue(LoadSceneRequest.Create(sceneName, args));
            }
        }

        public bool IsSceneLoaded(string sceneName)
        {
            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(sceneName));

            if (scene == null || !scene.isLoaded || !scene.IsValid())
            {
                return false;
            }

            return m_ListLoadedScene != null && m_ListLoadedScene.Contains(sceneName);
        }

        public bool IsSceneLoading(string sceneName)
        {
            return m_ListLoadingScene != null && m_ListLoadedScene.Contains(sceneName);
        }

        public bool IsSceneUnLoading(string sceneName)
        {
            return m_ListUnLoadingScene != null && m_ListUnLoadingScene.Contains(sceneName);
        }

        public void AllowScene()
        {
            if(m_AsyncOperation != null && !m_AsyncOperation.allowSceneActivation)
            {
                m_AsyncOperation.allowSceneActivation = true;
            }
        }

        private void InnerLoadSceneAsync(LoadSceneRequest request)
        {
            m_ListLoadingScene.Add(request.sceneName);

#if UNITY_EDITOR
            if (!AppConfig.instance.loadAB)
            {
                StartCoroutine(OnLoadSceneAsync(request));
            }
            else
#endif
            {
                AssetsMgr.instance.LoadAssetAsync(request.sceneName, OnLoadAssetComplete, typeof(UnityEngine.SceneManagement.Scene), request);
            }
        }

        private void OnLoadAssetComplete(string assetPath, UnityEngine.Object asset, object[] args)
        {
            LoadSceneRequest request = args[0] as LoadSceneRequest;
            StartCoroutine(OnLoadSceneAsync(request));
        }

        private IEnumerator OnLoadSceneAsync(LoadSceneRequest request)
        {
            try
            {
                LoadSceneParameters parameters = new LoadSceneParameters() { loadSceneMode = request.mode };

#if UNITY_EDITOR
                if(!AppConfig.instance.loadAB)
                {
                    m_AsyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(PathUtil.GetAssetFullPath(request.sceneName), parameters);
                }
                else
#endif
                {
                    m_AsyncOperation = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(request.sceneName), parameters);
                }

                m_AsyncOperation.allowSceneActivation = false;
            }
            catch(Exception e)
            {
                LoadSceneFailure(request.sceneName, e.Message, request.args);
                ReferencePool.ReleaseReference(request);
                yield break;
            }

            LoadSceneUpdateEventArgs updateEventArgs = LoadSceneUpdateEventArgs.Create(request.sceneName, 0);

            while (!m_AsyncOperation.isDone)
            {
                if (m_AsyncOperation.progress < 0.9f)
                    updateEventArgs.progress = m_AsyncOperation.progress;
                else
                    updateEventArgs.progress = 1.0f;

                m_LoadSceneUpdateEvent?.Invoke(updateEventArgs);

                if (updateEventArgs.progress >= 0.9)
                {
                    if (request.isAutoAllowScene)
                    {
                        m_AsyncOperation.allowSceneActivation = true;
                        yield return null;
                    }

                    LoadSceneSuccess(request.sceneName, request.args);
                }

                yield return null;
            }

            ReferencePool.ReleaseReference(updateEventArgs);
            ReferencePool.ReleaseReference(request);
        }

        private void InnerUnLoadSceneAsync(LoadSceneRequest request)
        {
#if UNITY_EDITOR
            if (!AppConfig.instance.loadAB)
            {
                StartCoroutine(OnUnLoadSceneAsync(request));
            }
            else
#endif
            {
                AssetsMgr.instance.UnloadAsset(request.sceneName);
                StartCoroutine(OnUnLoadSceneAsync(request));
            }
        }

        private IEnumerator OnUnLoadSceneAsync(LoadSceneRequest request)
        {
            try
            {
                m_ListUnLoadingScene.Add(request.sceneName);

#if UNITY_EDITOR
                if (!AppConfig.instance.loadAB)
                {
                    m_AsyncOperation = UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync(PathUtil.GetAssetFullPath(request.sceneName));
                }
                else
#endif
                {
                    m_AsyncOperation = SceneManager.UnloadSceneAsync(Path.GetFileNameWithoutExtension(request.sceneName));
                }
            }
            catch (Exception e)
            {
                UnLoadSceneFailure(request.sceneName, e.Message, request.args);
                ReferencePool.ReleaseReference(request);
                yield break;
            }

            while (!m_AsyncOperation.isDone)
            {
                yield return null;
            }

            UnLoadSceneSuccess(request.sceneName, request.args);
            ReferencePool.ReleaseReference(request);
        }

        private void LoadSceneSuccess(string sceneName, object[] args)
        {
            if(!string.IsNullOrEmpty(m_CurrSceneName) && !IsSceneLoaded(m_CurrSceneName))
            {
                m_ListLoadedScene.Remove(m_CurrSceneName);
            }

            m_CurrSceneName = sceneName;
            m_ListLoadingScene.Remove(sceneName);
            m_ListUnLoadedScene.Remove(sceneName);
            m_ListLoadedScene.Add(sceneName);

            LoadSceneSuccessEventArgs successEventArgs = LoadSceneSuccessEventArgs.Create(sceneName, args);
            m_LoadSceneSuccessEvent?.Invoke(successEventArgs);
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneUpdateEvent = null;
            m_LoadSceneFailureEvent = null;
            ReferencePool.ReleaseReference(successEventArgs);
        }

        private void LoadSceneFailure(string sceneName, string errorMessage, object[] args)
        {
            LoadSceneFailureEventArgs failureEventArgs = LoadSceneFailureEventArgs.Create(sceneName, errorMessage, args);
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneUpdateEvent = null;
            m_LoadSceneFailureEvent?.Invoke(failureEventArgs);
            m_LoadSceneFailureEvent = null;
            ReferencePool.ReleaseReference(failureEventArgs);
        }

        private void UnLoadSceneSuccess(string sceneName, object[] args)
        {
            m_ListUnLoadedScene.Add(sceneName);
            m_ListLoadedScene.Remove(sceneName);

            UnLoadSceneSuccessEventArgs successEventArgs = UnLoadSceneSuccessEventArgs.Create(sceneName, args);
            m_UnLoadSceneSuccessEvent?.Invoke(successEventArgs);
            m_UnLoadSceneSuccessEvent = null;
            ReferencePool.ReleaseReference(successEventArgs);
        }

        private void UnLoadSceneFailure(string sceneName, string errorMessage, object[] args)
        {
            UnLoadSceneFailureEventArgs failureEventArgs = UnLoadSceneFailureEventArgs.Create(sceneName, errorMessage, args);
            m_UnLoadSceneFailureEvent?.Invoke(failureEventArgs);
            m_UnLoadSceneFailureEvent = null;
            ReferencePool.ReleaseReference(failureEventArgs);
        }

        private string m_CurrSceneName = string.Empty;
        private List<string> m_ListLoadingScene = null;
        private List<string> m_ListUnLoadingScene = null;
        private List<string> m_ListLoadedScene = null;
        private List<string> m_ListUnLoadedScene = null;
        private Queue<LoadSceneRequest> m_LoadQueue = null;
        private AsyncOperation m_AsyncOperation = null;
        private GameFrameWorkAction<LoadSceneSuccessEventArgs> m_LoadSceneSuccessEvent = null;
        private GameFrameWorkAction<LoadSceneFailureEventArgs> m_LoadSceneFailureEvent = null;
        private GameFrameWorkAction<LoadSceneUpdateEventArgs> m_LoadSceneUpdateEvent = null;
        private GameFrameWorkAction<UnLoadSceneSuccessEventArgs> m_UnLoadSceneSuccessEvent = null;
        private GameFrameWorkAction<UnLoadSceneFailureEventArgs> m_UnLoadSceneFailureEvent = null;
    }
}