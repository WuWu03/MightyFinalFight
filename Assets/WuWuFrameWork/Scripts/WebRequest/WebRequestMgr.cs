using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WuWuFramework.Event;

namespace WuWuFramework.WebRequest
{
    public class WebRequestMgr : WuWuFrameworkModule, IWebRequestMgr
    {
        private readonly List<WebRequest> m_WebRequests;
        private const int MAX_DOING_REQUEST_COUNT = 10;
        private int m_MaxDoingRequestCount = 1;
        private int m_CurrDoingRequestCount = 0;

        public WebRequestMgr()
        {
            m_WebRequests = new List<WebRequest>(50);
            MonoBehaviourMgr.instance.updateEvent += Update;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="uri"></param>
        public void AddWebRequest(string uri)
        {
            AddWebRequest(uri, null, null, null);
        }

        /// <summary>
        /// Post请求，如果postData为空则进行Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="postData"></param>
        public void AddWebRequest(string uri, WWWForm postData)
        {
            AddWebRequest(uri, postData, null, null, null);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onRequestCompleteEvent"></param>
        /// <param name="onRequestProgressEvent"></param>
        /// <param name="onRequestErrorEvent"></param>
        public void AddWebRequest(string uri, WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent, WuWuFrameworkAction<float> onRequesProgressEvent, WuWuFrameworkAction<string> onRequestErrorEvent)
        {
            AddWebRequest(uri, null, onRequestCompleteEvent, onRequesProgressEvent, onRequestErrorEvent);
        }

        /// <summary>
        /// Post请求，如果postData为空则进行Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="postData"></param>
        /// <param name="onRequestCompleteEvent"></param>
        /// <param name="onRequestProgressEvent"></param>
        /// <param name="onRequestErrorEvent"></param>
        public void AddWebRequest(string uri, WWWForm postData, WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent, WuWuFrameworkAction<float> onRequesProgressEvent, WuWuFrameworkAction<string> onRequestErrorEvent)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new WuWuFrameworkException("无效的链接");
            }

            if (m_WebRequests is { Count: > 0 })
            {
                foreach (WebRequest temp in m_WebRequests)
                {
                    if (temp.uri == uri)
                    {
                        if (postData == null)
                        {
                            if (temp.postData == null)
                            {
                                throw new WuWuFrameworkException("重复的请求 :[" + uri + "]");
                            }
                        }
                        else if (temp.postData != null)
                        {
                            if (temp.postData.ToString() == postData.ToString())
                            {
                                throw new WuWuFrameworkException("重复的请求 :[" + uri + "]");
                            }
                        }
                    }
                }
            }

            WebRequest webRequest = WebRequest.Create(uri, postData);
            webRequest.onRequesProgressEvent += onRequesProgressEvent;
            webRequest.onRequestCompleteEvent += onRequestCompleteEvent;
            webRequest.onRequestErrorEvent += onRequestErrorEvent;
            m_WebRequests.Add(webRequest);
        }

        public void RemoveWebRequest(string uri)
        {
            if (m_WebRequests == null || m_WebRequests.Count == 0 || string.IsNullOrEmpty(uri))
            {
                return;
            }

            for (int i = m_WebRequests.Count - 1; i >= 0; i--)
            {
                WebRequest webRequest = m_WebRequests[i];

                if (m_WebRequests[i].uri == uri)
                {
                    webRequest.Release();
                    m_WebRequests.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllWebRequests()
        {
            if (m_WebRequests is { Count: > 0 })
            {
                foreach (WebRequest webRequest in m_WebRequests)
                {
                    webRequest.Release();
                }

                m_WebRequests.Clear();
            }
        }

        /// <summary>
        /// 设置同时进行的Web请求数量，最大不超过10个，默认1个
        /// </summary>
        /// <param name="maxRequsetCount"></param>
        public void SetMaxRequsetCount(int maxRequsetCount)
        {
            m_MaxDoingRequestCount = Math.Min(maxRequsetCount, MAX_DOING_REQUEST_COUNT);
        }

        public override void Shutdown()
        {
            RemoveAllWebRequests();
            m_WebRequests.Clear();
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (m_WebRequests is not { Count: > 0 })
            {
                return;
            }

            for (int i = m_WebRequests.Count - 1; i >= 0; i--)
            {
                WebRequest webRequest = m_WebRequests[i];

                if (webRequest.isDone || webRequest.isError)
                {
                    m_CurrDoingRequestCount--;
                    webRequest.Release();
                    m_WebRequests.RemoveAt(i);
                }
                else
                {
                    webRequest.Update();
                }
            }

            int maxRequestCount = Math.Min(m_WebRequests.Count, m_MaxDoingRequestCount);

            if (m_CurrDoingRequestCount >= maxRequestCount)
            {
                return;
            }

            for (int i = 0; i < m_WebRequests.Count; i++)
            {
                if (!m_WebRequests[i].isDoing)
                {
                    m_WebRequests[i].StartRequest();
                    m_CurrDoingRequestCount++;

                    if (m_CurrDoingRequestCount >= maxRequestCount)
                    {
                        break;
                    }
                }
            }
        }
    }
}
