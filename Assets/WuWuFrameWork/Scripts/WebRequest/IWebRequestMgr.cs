using WuWuFramework.Event;
using UnityEngine;
using UnityEngine.Networking;

namespace WuWuFramework.WebRequest
{
    public interface IWebRequestMgr
    {
        public void AddWebRequest(string uri, string tag);
        public void AddWebRequest(string uri, string tag, WWWForm postData);

        public void AddWebRequest(string uri, string tag,
            WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent,
            WuWuFrameworkAction<float> onRequestProgressEvent,
            WuWuFrameworkAction<string> onRequestErrorEvent);

        public void AddWebRequest(string uri, string tag, WWWForm postData,
            WuWuFrameworkAction<UnityWebRequest> onRequestCompleteEvent,
            WuWuFrameworkAction<float> onRequestProgressEvent,
            WuWuFrameworkAction<string> onRequestErrorEvent);
        public void RemoveWebRequest(string uri, string tag);
        public void RemoveWebRequestByUrl(string uri);
        public void RemoveWebRequestByTag(string tag);
        public void RemoveAllWebRequests();
    }
}
