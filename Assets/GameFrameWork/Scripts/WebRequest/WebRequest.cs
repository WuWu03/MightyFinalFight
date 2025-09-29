using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFrameWork.WebRequest
{
    public class WebRequest : IReference
    {
        public string uri { get; private set; }
        public string tag { get; private set; }
        public WWWForm postData { get; private set; }
        public bool isDoing { get; private set; }
        public bool isDone { get; private set; }
        public bool isError { get; private set; }

        public event GameFrameWorkAction<UnityWebRequest> onRequestCompleteEvent
        {
            add
            {
                m_OnRequestCompleteEvent += value;
            }
            remove
            {
                m_OnRequestCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<float> onRequesProgressEvent
        {
            add
            {
                m_OnRequesProgressEvent += value;
            }
            remove
            {
                m_OnRequesProgressEvent -= value;
            }
        }

        public event GameFrameWorkAction<string> onRequestErrorEvent
        {
            add
            {
                m_OnRequestErrorEvent += value;
            }
            remove
            {
                m_OnRequestErrorEvent -= value;
            }
        }

        public static WebRequest Create(MonoBehaviour monoBehaviour, string uri, string tag, WWWForm postData)
        {
            WebRequest webRequest = ReferencePool.Acquire<WebRequest>();
            webRequest.m_MonoBehaviour = monoBehaviour;
            webRequest.uri = uri;
            webRequest.tag = tag;
            webRequest.postData = postData;
            return webRequest;
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            uri = null;
            tag = null;
            postData = null;
            m_OnRequesProgressEvent = null;
            m_OnRequestErrorEvent = null;
            m_OnRequestCompleteEvent = null;
            m_MonoBehaviour = null;
        }

        public void StartRequest()
        {
            if (m_MonoBehaviour == null || isDoing || isDone || isError)
            {
                return;
            }

            isDoing = true;
            isDone = false;
            isError = false;
            m_MonoBehaviour.StartCoroutine(RequestCoroutine());
        }

        public void StopRequest()
        {
            if (m_MonoBehaviour == null || !isDoing)
            {
                return;
            }

            m_MonoBehaviour.StopCoroutine(RequestCoroutine());
            isDoing = false;
            isDone = false;
            isError = true;
        }

        private IEnumerator RequestCoroutine()
        {
            UnityWebRequest uwr = null;

            if (postData != null)
            {
                uwr = UnityWebRequest.Post(uri, postData);
            }
            else
            {
                uwr = UnityWebRequest.Get(uri);
            }

            if (uwr == null)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_OnRequestErrorEvent?.Invoke("请求失败，请检查链接是否正确");
                yield break;
            }

            uwr.timeout = 15;
            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = uwr.SendWebRequest();

            while (!unityWebRequestAsyncOperation.isDone)
            {
                m_OnRequesProgressEvent?.Invoke(unityWebRequestAsyncOperation.progress);
                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_OnRequestErrorEvent?.Invoke(uwr.error);
            }
            else if (uwr.result == UnityWebRequest.Result.Success)
            {
                m_OnRequesProgressEvent?.Invoke(1);
                yield return null;

                isDoing = false;
                isDone = true;
                isError = false;
                m_OnRequestCompleteEvent?.Invoke(uwr);
            }
        }

        private GameFrameWorkAction<UnityWebRequest> m_OnRequestCompleteEvent;
        private GameFrameWorkAction<float> m_OnRequesProgressEvent;
        private GameFrameWorkAction<string> m_OnRequestErrorEvent;
        private MonoBehaviour m_MonoBehaviour = null;
    }
}