using UnityEngine;
using UnityEngine.Networking;
using WuWuFramework.Event;

namespace WuWuFramework.WebRequest
{
    public class WebRequest : IReference
    {
        private WuWuFrameworkAction<UnityWebRequest> m_RequestCompleteEvent;
        private WuWuFrameworkAction<float> m_RequestProgressEvent;
        private WuWuFrameworkAction<string> m_RequestErrorEvent;
        private UnityWebRequest m_WebRequest;
        private UnityWebRequestAsyncOperation m_WebRequestAsyncOperation;

        public string uri { get; private set; }
        public WWWForm postData { get; private set; }
        public bool isDoing { get; private set; }
        public bool isDone { get; private set; }
        public bool isError { get; private set; }

        public event WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent
        {
            add
            {
                m_RequestCompleteEvent += value;
            }
            remove
            {
                m_RequestCompleteEvent -= value;
            }
        }

        public event WuWuFrameworkAction<float> onRequesProgressEvent
        {
            add
            {
                m_RequestProgressEvent += value;
            }
            remove
            {
                m_RequestProgressEvent -= value;
            }
        }

        public event WuWuFrameworkAction<string> onRequestErrorEvent
        {
            add
            {
                m_RequestErrorEvent += value;
            }
            remove
            {
                m_RequestErrorEvent -= value;
            }
        }

        public static WebRequest Create(string uri, WWWForm postData)
        {
            WebRequest webRequest = ReferencePool.Acquire<WebRequest>();
            webRequest.uri = uri;
            webRequest.postData = postData;
            return webRequest;
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            StopRequest();
            uri = null;
            postData = null;
            m_RequestProgressEvent = null;
            m_RequestErrorEvent = null;
            m_RequestCompleteEvent = null;
        }

        public void StartRequest()
        {
            if (isDoing || isDone || isError)
            {
                return;
            }

            isDoing = true;
            isDone = false;
            isError = false;
            m_WebRequest = postData != null ? UnityWebRequest.Post(uri, postData) : UnityWebRequest.Get(uri);

            if (m_WebRequest == null)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_RequestErrorEvent?.Invoke("请求失败，请检查链接是否正确");
                throw new WuWuFrameworkException("请求失败，请检查链接是否正确");
            }

            m_WebRequest.timeout = 15;
            m_WebRequestAsyncOperation = m_WebRequest.SendWebRequest();
        }

        public void StopRequest()
        {
            if (!isDoing)
            {
                return;
            }

            isDoing = false;
            isDone = false;
            isError = false;
            m_WebRequest?.Dispose();
            m_WebRequest = null;
            m_WebRequestAsyncOperation = null;
        }

        public void Update()
        {
            if (m_WebRequest == null || m_WebRequestAsyncOperation == null || !isDoing)
            {
                return;
            }

            if (!m_WebRequestAsyncOperation.isDone)
            {
                m_RequestProgressEvent?.Invoke(m_WebRequestAsyncOperation.progress);
                return;
            }

            switch (m_WebRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    isDoing = false;
                    isDone = false;
                    isError = true;
                    m_RequestErrorEvent?.Invoke(m_WebRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    m_RequestProgressEvent?.Invoke(1);
                    m_RequestCompleteEvent?.Invoke(m_WebRequest);
                    isDoing = false;
                    isDone = true;
                    isError = false;
                    break;
            }
        }
    }
}