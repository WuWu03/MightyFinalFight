using System.Collections;
using GameFrameWork.Event;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace GameFrameWork.Download
{
    public class DownloadRequest : IReference
    {
        public DownloadType downloadType { get; private set; }
        public string uri { get; private set; }
        public string tag { get; private set; }
        public string version { get; private set; }
        public ulong downloadSize { get; private set; }
        public bool isDoing { get; private set; }
        public bool isDone { get; private set; }
        public bool isError { get; private set; }

        public event GameFrameWorkAction<string, string, string, string, ulong> onDownloadScriptCompleteEvent
        {
            add
            {
                m_OnDownloadScriptCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadScriptCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, ulong> onDownloadBinaryFileCompleteEvent
        {
            add
            {
                m_OnDownloadBinaryFileCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadBinaryFileCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, Texture2D, ulong> onDownloadTextureCompleteEvent
        {
            add
            {
                m_OnDownloadTextureCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadTextureCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, AudioClip, ulong> onDownloadAudioClipCompleteEvent
        {
            add
            {
                m_OnDownloadAudioClipCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadAudioClipCompleteEvent -= value;
            }
        }
        public event GameFrameWorkAction<string, string, string, VideoClip, ulong> onDownloadVideoClipCompleteEvent
        {
            add
            {
                m_OnDownloadVideoClipCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadVideoClipCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, AssetBundle, ulong> onDownloadAssetBundleCompleteEvent
        {
            add
            {
                m_OnDownloadAssetBundleCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadAssetBundleCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent
        {
            add
            {
                m_OnDownloadProgressEvent += value;
            }
            remove
            {
                m_OnDownloadProgressEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent
        {
            add
            {
                m_OnDownloadErrorEvent += value;
            }
            remove
            {
                m_OnDownloadErrorEvent -= value;
            }
        }

        public static DownloadRequest Create(MonoBehaviour monoBehaviour, DownloadType downloadType, string uri, string tag, string version, ulong downloadSize)
        {
            DownloadRequest downloadRequest = ReferencePool.Acquire<DownloadRequest>();
            downloadRequest.m_MonoBehaviour = monoBehaviour;
            downloadRequest.downloadType = downloadType;
            downloadRequest.uri = uri;
            downloadRequest.tag = tag;
            downloadRequest.version = version;
            downloadRequest.downloadSize = downloadSize;
            return downloadRequest;
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            uri = null;
            tag = null;
            downloadSize = 0;
            isDoing = false;
            isDone = false;
            isError = false;
            m_OnDownloadScriptCompleteEvent = null;
            m_OnDownloadBinaryFileCompleteEvent = null;
            m_OnDownloadTextureCompleteEvent = null;
            m_OnDownloadAudioClipCompleteEvent = null;
            m_OnDownloadVideoClipCompleteEvent = null;
            m_OnDownloadAssetBundleCompleteEvent = null;
            StopDownload();
        }

        public void StartDownload()
        {
            if (m_MonoBehaviour == null || isDoing || isDone || isError)
            {
                return;
            }

            isDoing = true;
            isDone = false;
            isError = false;
            m_MonoBehaviour.StartCoroutine(DownloadCoroutine());
        }

        public void StopDownload()
        {
            if (m_MonoBehaviour == null || !isDoing)
            {
                return;
            }

            isDoing = false;
            isDone = false;
            isError = false;

            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Abort();
                m_UnityWebRequest.downloadHandler.Dispose();
                m_UnityWebRequest = null;
            }

            m_MonoBehaviour.StopCoroutine(DownloadCoroutine());
        }

        private IEnumerator DownloadCoroutine()
        {
            m_UnityWebRequest = CreateWebRequest();

            if (m_UnityWebRequest == null)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_OnDownloadErrorEvent?.Invoke(uri, tag, version, "请求失败，请检查链接是否正确");
                yield break;
            }

            ulong startDownloadLength = 0;

            if (m_UnityWebRequest.downloadHandler is DownloadHandlerFile downloadHandlerFile)
            {
                startDownloadLength = downloadHandlerFile.startDownloadLength;
            }

            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = m_UnityWebRequest.SendWebRequest();

            while (!unityWebRequestAsyncOperation.isDone)
            {
                try
                {
                    Log.LogInfo("当前进度：", (startDownloadLength + m_UnityWebRequest.downloadedBytes).ToString());
                    m_OnDownloadProgressEvent?.Invoke(uri, tag, version, startDownloadLength + m_UnityWebRequest.downloadedBytes, downloadSize);
                }
                catch
                {

                }

                yield return null;
            }

            if (m_UnityWebRequest.result == UnityWebRequest.Result.ConnectionError || m_UnityWebRequest.result == UnityWebRequest.Result.ProtocolError || m_UnityWebRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                OnDownloadError(m_UnityWebRequest.error);
            }
            else if (m_UnityWebRequest.result == UnityWebRequest.Result.Success)
            {
                OnDownloadComplete(downloadType, m_UnityWebRequest);
            }
        }

        private UnityWebRequest CreateWebRequest()
        {
            UnityWebRequest uwr = null;
            DownloadHandler downloadHandler = null;

            switch (downloadType)
            {
                case DownloadType.AssetBundle:
                    uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    downloadHandler = new DownloadHandlerAssetBundle(uri, 0);
                    break;
                case DownloadType.File:
                    uwr = UnityWebRequest.Get(uri);
                    downloadHandler = new DownloadHandlerFile(uri, version);
                    uwr.SetRequestHeader("Range", "bytes=" + (downloadHandler as DownloadHandlerFile).startDownloadLength.ToString() + "-");
                    Log.LogInfo("起始进度：", (downloadHandler as DownloadHandlerFile).startDownloadLength.ToString());
                    break;
                case DownloadType.Texture:
                    uwr = UnityWebRequestTexture.GetTexture(uri);
                    downloadHandler = new DownloadHandlerTexture(true);
                    break;
                case DownloadType.AudioClip:
                    uwr = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN);
                    downloadHandler = new DownloadHandlerAudioClip(uri, AudioType.UNKNOWN);
                    break;
                case DownloadType.VideoClip:
                    break;
                case DownloadType.Script:
                    uwr = UnityWebRequest.Get(uri);
                    downloadHandler = new DownloadHandlerScript();
                    break;
                case DownloadType.Buffer:
                    uwr = UnityWebRequest.Get(uri);
                    downloadHandler = new DownloadHandlerBuffer();
                    break;
            }

            if (downloadHandler != null)
            {
                uwr.downloadHandler = downloadHandler;
            }

            return uwr;
        }

        private void OnDownloadError(string errorMessage)
        {
            isDoing = false;
            isDone = false;
            isError = true;
            m_OnDownloadErrorEvent?.Invoke(uri, tag, version, errorMessage);
        }

        private void OnDownloadComplete(DownloadType downloadType, UnityWebRequest uwr)
        {
            isDoing = false;
            isDone = true;
            isError = false;

            switch (downloadType)
            {
                case DownloadType.AssetBundle:
                    OnAssetBundleDownloaded(uwr);
                    break;
                case DownloadType.File:
                    OnFileDownloaded();
                    break;
                case DownloadType.Texture:
                    OnTextureDownloaded(uwr);
                    break;
                case DownloadType.AudioClip:
                    OnAudioClipDownloaded(uwr);
                    break;
                case DownloadType.VideoClip:
                    //m_OnDownloadVideoClipCompleteEvent?.Invoke(DownloadHan.GetContent(uwr));
                    break;
                case DownloadType.Script:
                    OnScriptDownloaded(uwr);
                    break;
            }
        }

        private void OnFileDownloaded()
        {
            m_OnDownloadBinaryFileCompleteEvent?.Invoke(uri, tag, version, downloadSize);
        }

        private void OnAssetBundleDownloaded(UnityWebRequest uwr)
        {
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
            m_OnDownloadAssetBundleCompleteEvent?.Invoke(uri, tag, version, assetBundle, downloadSize);
        }

        private void OnScriptDownloaded(UnityWebRequest uwr)
        {
            m_OnDownloadScriptCompleteEvent?.Invoke(uri, tag, version, uwr.downloadHandler.text, downloadSize);
        }

        private void OnTextureDownloaded(UnityWebRequest uwr)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
            m_OnDownloadTextureCompleteEvent?.Invoke(uri, tag, version, texture, downloadSize);
        }

        private void OnAudioClipDownloaded(UnityWebRequest uwr)
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            m_OnDownloadAudioClipCompleteEvent?.Invoke(uri, tag, version, audioClip, downloadSize);
        }

        private GameFrameWorkAction<string, string, string, string, ulong> m_OnDownloadScriptCompleteEvent;
        private GameFrameWorkAction<string, string, string, ulong> m_OnDownloadBinaryFileCompleteEvent;
        private GameFrameWorkAction<string, string, string, Texture2D, ulong> m_OnDownloadTextureCompleteEvent;
        private GameFrameWorkAction<string, string, string, AudioClip, ulong> m_OnDownloadAudioClipCompleteEvent;
        private GameFrameWorkAction<string, string, string, VideoClip, ulong> m_OnDownloadVideoClipCompleteEvent;
        private GameFrameWorkAction<string, string, string, AssetBundle, ulong> m_OnDownloadAssetBundleCompleteEvent;
        private GameFrameWorkAction<string, string, string, ulong, ulong> m_OnDownloadProgressEvent;
        private GameFrameWorkAction<string, string, string, string> m_OnDownloadErrorEvent;
        private MonoBehaviour m_MonoBehaviour = null;
        private UnityWebRequest m_UnityWebRequest = null;
    }
}