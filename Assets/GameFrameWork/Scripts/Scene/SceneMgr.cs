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
                        StartCoroutine(InnerUnLoadSceneAsync(request));
                    else
                        StartCoroutine(InnerLoadSceneAsync(request));
                }
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();

            while (m_LoadQueue.Count > 0)
            {
                ReferencePool.Release(m_LoadQueue.Dequeue());
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

        public void LoadSceneAsync(string sceneName, params object[] args)
        {
            LoadSceneAsync(sceneName, LoadSceneMode.Single, true, args);
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, bool isAutoAllowScene, params object[] args)
        {
            if (isLoading)
            {
                LoadSceneFailure(sceneName, "SceneMgr is in loading.", null);
                return;
            }

            if (isUnLoading)
            {
                LoadSceneFailure(sceneName, "SceneMgr is in unloading.", null);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Format("Scene name:[", sceneName, "] is loaded."), null);
                return;
            }

            m_AsyncOperation = null;

            lock (m_LoadQueue)
            {
                m_LoadQueue.Enqueue(LoadSceneRequest.Create(sceneName, args, mode, isAutoAllowScene));
            }
        }

        public void LoadScene(string sceneName, params object[] args)
        {
            LoadScene(sceneName, LoadSceneMode.Single, args);
        }

        public void LoadScene(string sceneName, LoadSceneMode mode, object[] args)
        {
            if (isLoading)
            {
                LoadSceneFailure(sceneName, "SceneMgr is in loading.", args);
                return;
            }

            if (isUnLoading)
            {
                LoadSceneFailure(sceneName, "SceneMgr is in unloading.", null);
                return;
            }

            if (IsSceneLoaded(sceneName))
            {
                LoadSceneFailure(sceneName, StringUtil.Format("Scene name:[", sceneName, "] is loaded."), args);
                return;
            }

            try
            {
                SceneManager.LoadScene(sceneName, mode);
                LoadSceneSuccess(sceneName, null);
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
                UnLoadSceneFailure(sceneName, "SceneMgr is in loading.", args);
                return;
            }

            if (isUnLoading)
            {
                UnLoadSceneFailure(sceneName, "SceneMgr is in unloading.", null);
                return;
            }

            if (!IsSceneLoaded(sceneName))
            {
                UnLoadSceneFailure(sceneName, StringUtil.Format("Scene name:[", sceneName, "] is not loaded."), args);
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

        private IEnumerator InnerLoadSceneAsync(LoadSceneRequest request)
        {
            try
            {
                m_ListLoadingScene.Add(request.sceneName);
                m_AsyncOperation = SceneManager.LoadSceneAsync(request.sceneName, request.mode);
                m_AsyncOperation.allowSceneActivation = false;          
            }
            catch(Exception e)
            {
                LoadSceneFailure(request.sceneName, e.Message, request.args);
                ReferencePool.Release(request);
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

            ReferencePool.Release(updateEventArgs);
            ReferencePool.Release(request);
        }

        private IEnumerator InnerUnLoadSceneAsync(LoadSceneRequest request)
        {
            AsyncOperation ao = null;
            try
            {
                m_ListUnLoadingScene.Add(request.sceneName);
                ao = SceneManager.UnloadSceneAsync(request.sceneName);
            }
            catch (Exception e)
            {
                UnLoadSceneFailure(request.sceneName, e.Message, request.args);
                ReferencePool.Release(request);
                yield break;
            }

            while(!ao.isDone)
            {
                yield return null;
            }

            UnLoadSceneSuccess(request.sceneName, request.args);
            ReferencePool.Release(request);
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
            ReferencePool.Release(successEventArgs);
        }

        private void LoadSceneFailure(string sceneName, string errorMessage, object[] args)
        {
            LoadSceneFailureEventArgs failureEventArgs = LoadSceneFailureEventArgs.Create(sceneName, errorMessage, args);
            m_LoadSceneSuccessEvent = null;
            m_LoadSceneUpdateEvent = null;
            m_LoadSceneFailureEvent?.Invoke(failureEventArgs);
            m_LoadSceneFailureEvent = null;
            ReferencePool.Release(failureEventArgs);
        }

        private void UnLoadSceneSuccess(string sceneName, object[] args)
        {
            m_ListUnLoadedScene.Add(sceneName);
            m_ListLoadedScene.Remove(sceneName);

            UnLoadSceneSuccessEventArgs successEventArgs = UnLoadSceneSuccessEventArgs.Create(sceneName, args);
            m_UnLoadSceneSuccessEvent?.Invoke(successEventArgs);
            m_UnLoadSceneSuccessEvent = null;
            ReferencePool.Release(successEventArgs);
        }

        private void UnLoadSceneFailure(string sceneName, string errorMessage, object[] args)
        {
            UnLoadSceneFailureEventArgs failureEventArgs = UnLoadSceneFailureEventArgs.Create(sceneName, errorMessage, args);
            m_UnLoadSceneFailureEvent?.Invoke(failureEventArgs);
            m_UnLoadSceneFailureEvent = null;
            ReferencePool.Release(failureEventArgs);
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