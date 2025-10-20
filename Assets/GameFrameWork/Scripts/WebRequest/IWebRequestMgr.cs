using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFrameWork.WebRequest
{
    public interface IWebRequestMgr
    {
        public void AddWebRequest(string uri, string tag);
        public void AddWebRequest(string uri, string tag, WWWForm postData);

        public void AddWebRequest(string uri, string tag,
            GameFrameWorkAction<UnityWebRequest> onRequestCompleteEvent,
            GameFrameWorkAction<float> onRequestProgressEvent,
            GameFrameWorkAction<string> onRequestErrorEvent);

        public void AddWebRequest(string uri, string tag, WWWForm postData,
            GameFrameWorkAction<UnityWebRequest> onRequestCompleteEvent,
            GameFrameWorkAction<float> onRequestProgressEvent,
            GameFrameWorkAction<string> onRequestErrorEvent);
        public void RemoveWebRequest(string uri, string tag);
        public void RemoveWebRequestByUrl(string uri);
        public void RemoveWebRequestByTag(string tag);
        public void RemoveAllWebRequests();
    }
}
