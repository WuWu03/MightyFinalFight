using System.Collections.Generic;
using WuWuFramework.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace WuWuFramework.WebRequest
{
    public class WebRequestMgr : WuWuFrameworkModule,IWebRequestMgr
    {
        private readonly List<WebRequest> m_WebRequests;
        private readonly List<WebRequest> m_RemovedWebRequests;
        public WebRequestMgr()
        {
            m_WebRequests = new List<WebRequest>();
            m_RemovedWebRequests = new List<WebRequest>();
        }

        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (m_WebRequests is { Count: > 0 })
            {
                foreach (WebRequest webRequest in m_WebRequests)
                {
                    if (webRequest.isDone || webRequest.isError)
                    {
                        m_RemovedWebRequests.Add(webRequest);
                    }
                }
            }

            if (m_RemovedWebRequests is { Count: > 0 })
            {
                foreach (WebRequest webRequest in m_RemovedWebRequests)
                {
                    RemoveWebRequest(webRequest);
                }
                
                m_RemovedWebRequests.Clear();
            }

            if (m_WebRequests is { Count: > 0 } && !m_WebRequests[0].isDoing)
            {
                m_WebRequests[0].StartRequest();
            }
        }

        public override void Shutdown()
        {
            RemoveAllWebRequests();
            m_WebRequests.Clear();
        }
        
        public void AddWebRequest(string uri, string tag)
        {
            AddWebRequest(uri, tag, null, null, null);
        }

        public void AddWebRequest(string uri, string tag, WWWForm postData)
        {
            AddWebRequest(uri, tag, postData, null, null, null);
        }

        public void AddWebRequest(string uri, string tag, WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent, WuWuFrameworkAction<float> onRequesProgressEvent, WuWuFrameworkAction<string> onRequestErrorEvent)
        {
            AddWebRequest(uri, tag, null, onRequestCompleteEvent, onRequesProgressEvent, onRequestErrorEvent);
        }

        public void AddWebRequest(string uri, string tag, WWWForm postData, WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent, WuWuFrameworkAction<float> onRequesProgressEvent, WuWuFrameworkAction<string> onRequestErrorEvent)
        {
            WebRequest webRequest = WebRequest.Create(uri, tag, postData);
            webRequest.onRequesProgressEvent += onRequesProgressEvent;
            webRequest.onRequestCompleteEvent += onRequestCompleteEvent;
            webRequest.onRequestErrorEvent += onRequestErrorEvent;
            m_WebRequests.Add(webRequest);
        }

        public void RemoveWebRequest(string uri, string tag)
        {
            if (m_WebRequests == null || m_WebRequests.Count == 0 || string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(tag))
            {
                return;
            }

            for (int i = m_WebRequests.Count - 1; i >= 0; i--)
            {
                if (m_WebRequests[i].uri == uri && m_WebRequests[i].tag == tag)
                {
                    RemoveWebRequest(m_WebRequests[i]);
                    break;
                }
            }
        }

        public void RemoveWebRequestByUrl(string uri)
        {
            if (m_WebRequests == null || m_WebRequests.Count == 0 || string.IsNullOrEmpty(uri))
            {
                return;
            }

            for (int i = m_WebRequests.Count - 1; i >= 0; i--)
            {
                if (m_WebRequests[i].uri == uri)
                {
                    RemoveWebRequest(m_WebRequests[i]);
                }
            }
        }

        public void RemoveWebRequestByTag(string tag)
        {
            if (m_WebRequests == null || m_WebRequests.Count == 0 || string.IsNullOrEmpty(tag))
            {
                return;
            }

            for (int i = m_WebRequests.Count - 1; i >= 0; i--)
            {
                if (m_WebRequests[i].tag == tag)
                {
                    RemoveWebRequest(m_WebRequests[i]);
                }
            }
        }

        public void RemoveAllWebRequests()
        {
            if (m_WebRequests == null || m_WebRequests.Count == 0)
            {
                return;
            }

            for (int i = m_WebRequests.Count - 1; i >= 0; i--)
            {
                RemoveWebRequest(m_WebRequests[i]);
            }
        }

        private void RemoveWebRequest(WebRequest webRequest)
        {
            webRequest.StopRequest();
            webRequest.Release();
            m_WebRequests.Remove(webRequest);
        }
    }
}
