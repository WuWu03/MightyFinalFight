using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFrameWork.Scene
{
    public class LoadSceneRequest : BaseEventArgs
    {
        public string sceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public object[] args
        {
            get
            {
                return m_Args;
            }
        }

        public LoadSceneMode mode
        {
            get
            {
                return m_Mode;
            }
        }

        public bool isAutoAllowScene
        {
            get
            {
                return m_IsAutoAllowScene;
            }
        }

        public bool isUnLoad
        {
            get
            {
                return m_IsUnLoad;
            }
        }

        public static LoadSceneRequest Create(string sceneName, object[] args, LoadSceneMode mode, bool isAutoAllowScene)
        {
            LoadSceneRequest request = ReferencePool.Acquire<LoadSceneRequest>();
            request.m_SceneName = sceneName;
            request.m_Args = args;
            request.m_Mode = mode;
            request.m_IsAutoAllowScene = isAutoAllowScene;
            request.m_IsUnLoad = false;
            return request;
        }

        public static LoadSceneRequest Create(string sceneName, object[] args)
        {
            LoadSceneRequest request = ReferencePool.Acquire<LoadSceneRequest>();
            request.m_SceneName = sceneName;
            request.m_Args = args;
            request.m_IsUnLoad = true;
            return request;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_Args = null;
            m_IsAutoAllowScene = false;
        }

        private string m_SceneName = string.Empty;
        private object[] m_Args = null;
        private LoadSceneMode m_Mode;
        private bool m_IsAutoAllowScene = false;
        private bool m_IsUnLoad = false;
    }
}
