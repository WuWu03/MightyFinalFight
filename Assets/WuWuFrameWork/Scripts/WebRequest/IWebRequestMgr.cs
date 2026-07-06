using UnityEngine;
using UnityEngine.Networking;
using WuWuFramework.Event;

namespace WuWuFramework.WebRequest
{
    public interface IWebRequestMgr
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="uri"></param>
        public void AddWebRequest(string uri);

        /// <summary>
        /// Post请求，如果postData为空则进行Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="postData"></param>
        public void AddWebRequest(string uri, WWWForm postData);

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onRequestCompleteEvent"></param>
        /// <param name="onRequestProgressEvent"></param>
        /// <param name="onRequestErrorEvent"></param>
        public void AddWebRequest(string uri,
            WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent,
            WuWuFrameworkAction<float> onRequestProgressEvent,
            WuWuFrameworkAction<string> onRequestErrorEvent);

        /// <summary>
        /// Post请求，如果postData为空则进行Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="postData"></param>
        /// <param name="onRequestCompleteEvent"></param>
        /// <param name="onRequestProgressEvent"></param>
        /// <param name="onRequestErrorEvent"></param>
        public void AddWebRequest(string uri, WWWForm postData,
            WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent,
            WuWuFrameworkAction<float> onRequestProgressEvent,
            WuWuFrameworkAction<string> onRequestErrorEvent);

        /// <summary>
        /// 移除Web请求
        /// </summary>
        /// <param name="uri"></param>
        public void RemoveWebRequest(string uri);

        /// <summary>
        /// 移除所有Web请求
        /// </summary>
        public void RemoveAllWebRequests();
    }
}
