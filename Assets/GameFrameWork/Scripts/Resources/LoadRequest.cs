using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Resources
{
    public class LoadRequest
    {
        public bool loadMainAsset { get; set; }
        public Type assetType { get; set; }
        public string assetName { get; set; }
        public string assetPath 
        {
            get 
            {
                return m_AssetPath;
            }
        }

        public LoadRequest(string assetPath,GameFrameWorkAction<string, UnityEngine.Object, object[]> callback, object[] args)
        {
            m_AssetPath = assetPath;
            m_Callback = callback;
            m_Args = args;
        }

        public bool Call(UnityEngine.Object go)
        {
            if (m_Callback != null)
            {
                m_Callback?.Invoke(assetPath, go, m_Args);
                return true;
            }

            return false;
        }

        private string m_AssetPath = string.Empty;
        private GameFrameWorkAction<string, UnityEngine.Object, object[]> m_Callback;
        private object[] m_Args = null;
    }
}