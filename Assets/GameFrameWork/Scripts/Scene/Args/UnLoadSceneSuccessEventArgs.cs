using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Scene
{
    public class UnLoadSceneSuccessEventArgs : BaseEventArgs
    {
        public string sceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public object arg
        {
            get
            {
                return m_Arg;
            }
        }

        public static UnLoadSceneSuccessEventArgs Create(string sceneName, object arg)
        {
            UnLoadSceneSuccessEventArgs successEventArgs = ReferencePool.Acquire<UnLoadSceneSuccessEventArgs>();
            successEventArgs.m_SceneName = sceneName;
            successEventArgs.m_Arg = arg;
            return successEventArgs;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_Arg = null;
        }

        private object m_Arg = null;
        private string m_SceneName = string.Empty;
    }
}
