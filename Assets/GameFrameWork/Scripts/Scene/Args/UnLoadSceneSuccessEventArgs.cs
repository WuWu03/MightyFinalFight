using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Scene
{
    public class UnLoadSceneSuccessEventArgs : BaseEventArgs
    {
        public string SceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public object Args
        {
            get
            {
                return m_Args;
            }
        }

        public static UnLoadSceneSuccessEventArgs Create(string sceneName, object args)
        {
            UnLoadSceneSuccessEventArgs successEventArgs = ReferencePool.Acquire<UnLoadSceneSuccessEventArgs>();
            successEventArgs.m_SceneName = sceneName;
            successEventArgs.m_Args = args;
            return successEventArgs;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_Args = null;
        }

        private object m_Args = null;
        private string m_SceneName = string.Empty;
    }
}
