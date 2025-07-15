using GameFrameWork.Assets;
using GameFrameWork.Utils;
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

        public bool isLoading
        {
            get
            {
                return m_ListLoadingScene != null && m_ListLoadingScene.Count > 0;
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

        protected override void OnAwake()
        {
            base.OnAwake();
            m_ListLoadingScene = new List<string>();
            m_ListLoadedScene = new List<string>();
            m_LoadQueue = new Queue<LoadSceneRequest>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (isLoading)
            {
                return;
            }

            if (m_LoadQueue.Count > 0)
            {
                lock (m_LoadQueue)
                {
                    LoadSceneRequest request = m_LoadQueue.Dequeue();
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
            m_ListLoadedScene.Clear();

            m_CurrSceneName = string.Empty;
            m_AsyncOperation = null;
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneFailureEvent = null;
            m_LoadSceneUpdateEvent = null;
        }

        public void LoadSceneAsync(string sceneName)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true);
        }

        public void LoadSceneAsync(string sceneName, bool isAutoAllowScene)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, isAutoAllowScene);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene)
        {
            LoadSceneAsync(sceneName, mode, isAutoAllowScene, null);
        }

        public void LoadSceneAsync(string sceneName, object[] args)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true, args);
        }

        public void LoadSceneAsync(string sceneName, bool isAutoAllowScene, object[] args)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true, args);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene, object[] args)
        {
            if (isLoading)
            {
                LoadSceneFailure(sceneName, "加载失败，正在加载中，无法进行加载", null);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Append("加载失败，场景 : [", sceneName, "] 已加载"), null);
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

        public void LoadScene(string sceneName, LoadSceneMode mode)
        {
            LoadScene(sceneName, mode, null);
        }

        public void LoadScene(string sceneName, object[] args)
        {
            LoadScene(sceneName, LoadSceneMode.Single, args);
        }

        public void LoadScene(string sceneName, LoadSceneMode mode, object[] args)
        {
            if (IsSceneLoading(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Append("加载失败，场景 : [", sceneName, "] 正在加载"), args);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Append("加载失败，场景 : [", sceneName, "] 已加载"), args);
                return;
            }

            try
            {
                LoadSceneParameters parameters = new LoadSceneParameters() { loadSceneMode = mode };
#if UNITY_EDITOR
                if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(PathUtil.GetAssetPath(sceneName), parameters);
                }
                else
#endif
                {
                    AssetsMgr.instance.LoadAssetSync(sceneName, typeof(UnityEngine.SceneManagement.Scene));
                    SceneManager.LoadScene(sceneName, mode);
                }

                LoadSceneSuccess(sceneName, args);
            }
            catch (Exception e)
            {
                LoadSceneFailure(sceneName, e.Message, args);
            }
        }

        public void UnLoadScene(string sceneName, params object[] args)
        {
            m_ListLoadedScene.Remove(sceneName);

#if UNITY_EDITOR
            if (GameFrameWorkEntry.config.isLoadFromAssetBundle)
#endif
            {
                AssetsMgr.instance.UnloadAsset(sceneName);
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
            return m_ListLoadingScene != null && m_ListLoadingScene.Contains(sceneName);
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
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
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
                if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
                {
                    m_AsyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(PathUtil.GetAssetPath(request.sceneName), parameters);
                }
                else
#endif
                {
                    m_AsyncOperation = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(request.sceneName), parameters);
                }

                m_AsyncOperation.allowSceneActivation = false;
            }
            catch (Exception e)
            {
                LoadSceneFailure(request.sceneName, e.Message, request.args);
                ReferencePool.ReleaseReference(request);
                yield break;
            }

            LoadSceneUpdateEventArgs updateEventArgs = LoadSceneUpdateEventArgs.Create(request.sceneName, 0);

            while (!m_AsyncOperation.isDone)
            {
                if (m_AsyncOperation.progress < 0.9f)
                {
                    updateEventArgs.progress = m_AsyncOperation.progress;
                }
                else
                {
                    updateEventArgs.progress = 1.0f;
                }

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

        private void LoadSceneSuccess(string sceneName, object[] args)
        {
            if (m_CurrSceneName == sceneName)
            {
                return;
            }

            m_CurrSceneName = sceneName;
            m_ListLoadingScene.Remove(sceneName);
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

        private string m_CurrSceneName = string.Empty;
        private List<string> m_ListLoadingScene = null;
        private List<string> m_ListLoadedScene = null;
        private Queue<LoadSceneRequest> m_LoadQueue = null;
        private AsyncOperation m_AsyncOperation = null;
        private GameFrameWorkAction<LoadSceneSuccessEventArgs> m_LoadSceneSuccessEvent = null;
        private GameFrameWorkAction<LoadSceneFailureEventArgs> m_LoadSceneFailureEvent = null;
        private GameFrameWorkAction<LoadSceneUpdateEventArgs> m_LoadSceneUpdateEvent = null;
    }
}