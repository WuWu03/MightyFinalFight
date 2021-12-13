using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFrameWork.Utilities
{
    public class DownLoadUtil
    {

        /// <summary>
        /// httpœ¬‘ÿ
        /// </summary>
        /// <returns></returns>
        public static void WebRequest(string url, GameFrameWorkAction<UnityWebRequest> call, GameFrameWorkAction<string> error, GameFrameWorkAction<float> progressCall = null)
        {
            if (m_MonoBehaviour == null)
            {
                return;
            }

            m_MonoBehaviour.StartCoroutine(StartUnityWebRequest(url, call, error, progressCall));
        }

        //uwrœ¬‘ÿ
        private static IEnumerator StartUnityWebRequest(string url, GameFrameWorkAction<UnityWebRequest> call, GameFrameWorkAction<string> error, GameFrameWorkAction<float> progressCall)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                if (error != null) error(uwr.error);
            }
            else
            {
                while (!uwr.isDone)
                {
                    if (progressCall != null) progressCall(uwr.downloadProgress);
                    yield return null;
                }

                if (uwr.isDone)
                {
                    if (call != null) call(uwr);
                }
            }
        }

        private static MonoBehaviour m_MonoBehaviour = null;

    }
}