using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Scene
{
    public class LoadSceneFailureEventArgs : BaseEventArgs
    {
        public string SceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return m_ErrorMessage;
            }
        }

        public object Args
        {
            get
            {
                return m_Args;
            }
        }

        public static LoadSceneFailureEventArgs Create(string sceneName,string errorMessage,object args)
        {
            LoadSceneFailureEventArgs failureEventArgs = ReferencePool.Acquire<LoadSceneFailureEventArgs>();
            failureEventArgs.m_SceneName = sceneName;
            failureEventArgs.m_ErrorMessage = errorMessage;
            failureEventArgs.m_Args = args;
            return failureEventArgs;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_ErrorMessage = string.Empty;
            m_Args = null;
        }

        private string m_SceneName = string.Empty;
        private string m_ErrorMessage = string.Empty;
        private object m_Args = null;
    }
}