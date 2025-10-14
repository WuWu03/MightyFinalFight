using System.Collections.Generic;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFrameWork.WebRequest
{
    public class WebRequestMgr : BaseMgr<WebRequestMgr>
    {
        protected override void OnAwake()
        {
            m_WebRequests = new List<WebRequest>();
        }

        protected override void OnFixedUpdate()
        {
            if (m_WebRequests != null && m_WebRequests.Count > 0)
            {
                for (int i = m_WebRequests.Count - 1; i > -1; i--)
                {
                    if (m_WebRequests[i].isDone || m_WebRequests[i].isError)
                    {
                        RemoveWebRequest(m_WebRequests[i]);
                    }
                }
            }

            if (m_WebRequests != null && m_WebRequests.Count > 0 && !m_WebRequests[0].isDoing)
            {
                m_WebRequests[0].StartRequest();
            }
        }

        protected override void OnShutDown()
        {
            RemoveAllWebRequests();
            m_WebRequests.Clear();
        }

        protected override void OnDestory()
        {
            m_WebRequests = null;
        }

        public void AddWebRequest(string uri, string tag)
        {
            AddWebRequest(uri, tag, null, null, null);
        }

        public void AddWebRequest(string uri, string tag, WWWForm postData)
        {
            AddWebRequest(uri, tag, postData, null, null, null);
        }

        public void AddWebRequest(string uri, string tag, GameFrameWorkAction<UnityWebRequest> onRequestCompleteEvent, GameFrameWorkAction<float> onRequesProgressEvent, GameFrameWorkAction<string> onRequestErrorEvent)
        {
            AddWebRequest(uri, tag, null, onRequestCompleteEvent, onRequesProgressEvent, onRequestErrorEvent);
        }

        public void AddWebRequest(string uri, string tag, WWWForm postData, GameFrameWorkAction<UnityWebRequest> onRequestCompleteEvent, GameFrameWorkAction<float> onRequesProgressEvent, GameFrameWorkAction<string> onRequestErrorEvent)
        {
            WebRequest webRequest = WebRequest.Create(this, uri, tag, postData);
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

        private List<WebRequest> m_WebRequests = null;
    }
}
