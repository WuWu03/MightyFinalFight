using GameFrameWork.Assets;
using GameFrameWork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFrameWork.Scene
{
    public class SceneMgr : GameFrameWorkModule, ISceneMgr
    {
        private event GameFrameWorkAction<LoadSceneSuccessEventArgs> m_LoadSceneSuccessEvent;
        private event GameFrameWorkAction<LoadSceneFailureEventArgs> m_LoadSceneFailureEvent;
        private event GameFrameWorkAction<LoadSceneUpdateEventArgs> m_LoadSceneUpdateEvent;
        private readonly List<string> m_LoadingScenes;
        private readonly List<string> m_LoadedScenes;
        private readonly Queue<LoadSceneRequest> m_LoadRequests;
        private string m_CurrSceneName;
        private IResourceMgr m_ResourceMgr;
        private AsyncOperation m_AsyncOperation;
        
        public SceneMgr()
        {
            m_LoadingScenes = new();
            m_LoadedScenes = new();
            m_LoadRequests = new();
        }

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
                return m_LoadingScenes != null && m_LoadingScenes.Count > 0;
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
                return m_LoadedScenes.Count;
            }
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (isLoading)
            {
                return;
            }

            if (m_LoadRequests.Count > 0)
            {
                lock (m_LoadRequests)
                {
                    LoadSceneRequest request = m_LoadRequests.Dequeue();
                    InnerLoadSceneAsync(request);
                }
            }
        }

        public override void Shutdown()
        {
            while (m_LoadRequests.Count > 0)
            {
                m_LoadRequests.Dequeue().Release();
            }

            m_CurrSceneName = string.Empty;
            m_LoadingScenes.Clear();
            m_LoadedScenes.Clear();
            m_LoadRequests.Clear();
        }

        public void SetResourceMgr(IResourceMgr resourceMgr)
        {
            m_ResourceMgr = resourceMgr;
        }

        public void LoadSceneAsync(string sceneName, object arg = null)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true, arg);
        }

        public void LoadSceneAsync(string sceneName, bool isAutoAllowScene, object arg = null)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, isAutoAllowScene, arg);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene, object arg = null)
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
            m_LoadRequests.Enqueue(LoadSceneRequest.Create(sceneName, mode, isAutoAllowScene, arg));
        }

        public void LoadScene(string sceneName, object arg = null)
        {
            LoadScene(sceneName, LoadSceneMode.Single, arg);
        }

        public void LoadScene(string sceneName, LoadSceneMode mode, object arg = null)
        {
            if (IsSceneLoading(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Append("加载失败，场景 : [", sceneName, "] 正在加载"), arg);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Append("加载失败，场景 : [", sceneName, "] 已加载"), arg);
                return;
            }

            try
            {
                LoadSceneParameters parameters = new() { loadSceneMode = mode };
#if UNITY_EDITOR
                if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(PathUtil.GetAssetPath(sceneName),
                        parameters);
                }
                else
#endif
                {
                    m_ResourceMgr.Load(sceneName, typeof(UnityEngine.SceneManagement.Scene));
                    SceneManager.LoadScene(sceneName, mode);
                }

                LoadSceneSuccess(sceneName, arg);
            }
            catch (Exception e)
            {
                LoadSceneFailure(sceneName, e.Message, arg);
            }
        }

        public void UnLoadScene(string sceneName, params object[] args)
        {
            m_LoadedScenes.Remove(sceneName);

#if UNITY_EDITOR
            if (GameFrameWorkEntry.config.isLoadFromAssetBundle)
#endif
            {
                m_ResourceMgr.Unload(sceneName);
            }
        }

        public bool IsSceneLoaded(string sceneName)
        {
            UnityEngine.SceneManagement.Scene scene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(sceneName));

            if (!scene.isLoaded || !scene.IsValid())
            {
                return false;
            }

            return m_LoadedScenes != null && m_LoadedScenes.Contains(sceneName);
        }

        public bool IsSceneLoading(string sceneName)
        {
            return m_LoadingScenes != null && m_LoadingScenes.Contains(sceneName);
        }

        public void AllowScene()
        {
            if (m_AsyncOperation is { allowSceneActivation: false })
            {
                m_AsyncOperation.allowSceneActivation = true;
            }
        }

        private void InnerLoadSceneAsync(LoadSceneRequest request)
        {
            m_LoadingScenes.Add(request.sceneName);

#if UNITY_EDITOR
            if (!GameFrameWorkEntry.config.isLoadFromAssetBundle)
            {
                MonoBehaviourMgr.instance.StartCoroutine(OnLoadSceneAsync(request));
            }
            else
#endif
            {
                m_ResourceMgr.LoadAsync(request.sceneName, typeof(UnityEngine.SceneManagement.Scene), OnLoadAssetComplete, request);
            }
        }

        private void OnLoadAssetComplete(string assetPath, UnityEngine.Object asset, object arg)
        {
            LoadSceneRequest request = arg as LoadSceneRequest;
            MonoBehaviourMgr.instance.StartCoroutine(OnLoadSceneAsync(request));
        }

        private IEnumerator OnLoadSceneAsync(LoadSceneRequest request)
        {
            try
            {
                LoadSceneParameters parameters = new() { loadSceneMode = request.mode };

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
                LoadSceneFailure(request.sceneName, e.Message, request.arg);
                request.Release();
                yield break;
            }

            LoadSceneUpdateEventArgs updateEventArgs = LoadSceneUpdateEventArgs.Create(request.sceneName, 0);

            while (!m_AsyncOperation.isDone)
            {
                updateEventArgs.progress = m_AsyncOperation.progress < 0.9f ? m_AsyncOperation.progress : 1.0f;
                m_LoadSceneUpdateEvent?.Invoke(updateEventArgs);

                if (updateEventArgs.progress >= 0.9)
                {
                    if (request.isAutoAllowScene)
                    {
                        m_AsyncOperation.allowSceneActivation = true;
                        yield return null;
                    }

                    LoadSceneSuccess(request.sceneName, request.arg);
                }

                yield return null;
            }

            updateEventArgs.Release();
            request.Release();
        }

        private void LoadSceneSuccess(string sceneName, object arg)
        {
            if (m_CurrSceneName == sceneName)
            {
                return;
            }

            m_CurrSceneName = sceneName;
            m_LoadingScenes.Remove(sceneName);
            m_LoadedScenes.Add(sceneName);

            LoadSceneSuccessEventArgs successEventArgs = LoadSceneSuccessEventArgs.Create(sceneName, arg);
            m_LoadSceneSuccessEvent?.Invoke(successEventArgs);
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneUpdateEvent = null;
            m_LoadSceneFailureEvent = null;
            successEventArgs.Release();
        }

        private void LoadSceneFailure(string sceneName, string errorMessage, object arg)
        {
            LoadSceneFailureEventArgs failureEventArgs = LoadSceneFailureEventArgs.Create(sceneName, errorMessage, arg);
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneUpdateEvent = null;
            m_LoadSceneFailureEvent?.Invoke(failureEventArgs);
            m_LoadSceneFailureEvent = null;
            failureEventArgs.Release();
        }
    }
}