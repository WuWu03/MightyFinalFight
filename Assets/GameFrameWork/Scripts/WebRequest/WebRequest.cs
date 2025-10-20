using System;
using System.Collections;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFrameWork.WebRequest
{
    public class WebRequest : IReference
    {
        private GameFrameWorkAction<UnityWebRequest> m_RequestCompleteEvent;
        private GameFrameWorkAction<float> m_RequestProgressEvent;
        private GameFrameWorkAction<string> m_RequestErrorEvent;
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
                m_RequestCompleteEvent += value;
            }
            remove
            {
                m_RequestCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<float> onRequesProgressEvent
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

        public event GameFrameWorkAction<string> onRequestErrorEvent
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

        public static WebRequest Create(string uri, string tag, WWWForm postData)
        {
            WebRequest webRequest = ReferencePool.Acquire<WebRequest>();
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
            MonoBehaviourMgr.instance.StartCoroutine(RequestCoroutine());
        }

        public void StopRequest()
        {
            if (!isDoing)
            {
                return;
            }

            MonoBehaviourMgr.instance.StopCoroutine(RequestCoroutine());
            isDoing = false;
            isDone = false;
            isError = true;
        }

        private IEnumerator RequestCoroutine()
        {
            UnityWebRequest uwr = postData != null ? UnityWebRequest.Post(uri, postData) : UnityWebRequest.Get(uri);

            if (uwr == null)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_RequestErrorEvent?.Invoke("请求失败，请检查链接是否正确");
                throw new Exception("请求失败，请检查链接是否正确");
            }

            uwr.timeout = 15;
            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = uwr.SendWebRequest();

            while (!unityWebRequestAsyncOperation.isDone)
            {
                m_RequestProgressEvent?.Invoke(unityWebRequestAsyncOperation.progress);
                yield return null;
            }
            
            switch (uwr.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    isDoing = false;
                    isDone = false;
                    isError = true;
                    m_RequestErrorEvent?.Invoke(uwr.error);
                    break;
                case UnityWebRequest.Result.Success:
                    m_RequestProgressEvent?.Invoke(1);
                    yield return null;
                    isDoing = false;
                    isDone = true;
                    isError = false;
                    m_RequestCompleteEvent?.Invoke(uwr);
                    break;
            }
        }
    }
}