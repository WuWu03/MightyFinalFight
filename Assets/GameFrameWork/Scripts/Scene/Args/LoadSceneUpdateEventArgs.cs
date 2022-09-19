using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Scene
{
    public class LoadSceneUpdateEventArgs : BaseEventArgs
    {
        public string sceneName
        {
            get
            {
                return m_SceneName;
            }
        }

        public float progress
        {
            get
            {
                return m_Progress;
            }
            set
            {
                m_Progress = value;
            }
        }

        public static LoadSceneUpdateEventArgs Create(string sceneName, float progress)
        {
            LoadSceneUpdateEventArgs updateEventArgs = ReferencePool.Acquire<LoadSceneUpdateEventArgs>();
            updateEventArgs.m_SceneName = sceneName;
            updateEventArgs.m_Progress = progress;
            return updateEventArgs;
        }

        public override void Clear()
        {
            m_SceneName = string.Empty;
            m_Progress = 0f;
        }

        private string m_SceneName = string.Empty;
        private float m_Progress = 0f;
    }
}
