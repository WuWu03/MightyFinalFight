using GameFrameWork.Assets;
using System.Collections;
using UnityEngine.Networking;

namespace GameFrameWork.Utils
{
    public class DownLoadUtil
    {
        /// <summary>
        /// http下载
        /// </summary>
        /// <returns></returns>
        public static void WebRequest(string url, GameFrameWorkAction<UnityWebRequest> onComplete, GameFrameWorkAction<string> onError, GameFrameWorkAction<float> onProgress = null)
        {
            AssetsMgr.instance.StartCoroutine(StartUnityWebRequest(url, onComplete, onError, onProgress));
        }

        //uwr下载
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