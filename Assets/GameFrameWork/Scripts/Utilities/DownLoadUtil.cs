using System.Collections;
using UnityEngine.Networking;

namespace GameFrameWork.Utilities
{
    public class DownLoadUtil
    {
        /// <summary>
        /// httpœ¬‘ÿ
        /// </summary>
        /// <returns></returns>
        public static void WebRequest(string url, GameFrameWorkAction<UnityWebRequest> onComplete, GameFrameWorkAction<string> onError, GameFrameWorkAction<float> onProgress = null)
        {
            AppConfig.instance.StartCoroutine(StartUnityWebRequest(url, onComplete, onError, onProgress));
        }

        //uwrœ¬‘ÿ
        private static IEnumerator StartUnityWebRequest(string url, GameFrameWorkAction<UnityWebRequest> onComplete, GameFrameWorkAction<string> onError, GameFrameWorkAction<float> onProgress)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(url);
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                if (onError != null) onError(uwr.error);
            }
            else
            {
                while (!uwr.isDone)
                {
                    if (onProgress != null)
                    {
                        onProgress(uwr.downloadProgress);
                    }

                    yield return null;
                }

                if (uwr.isDone)
                {
                    if (onComplete != null)
                    {
                        onComplete(uwr);
                    }
                }
            }
        }
    }
}