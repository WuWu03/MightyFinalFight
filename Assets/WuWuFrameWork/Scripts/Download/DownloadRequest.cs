using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using WuWuFramework.Event;

namespace WuWuFramework.Download
{
    public class DownloadRequest : IReference
    {
        private WuWuFrameworkAction<string, string, ulong> m_OnDownloadTextCompleteEvent;
        private WuWuFrameworkAction<string, string, ulong> m_OnDownloadScriptCompleteEvent;
        private WuWuFrameworkAction<string, string, ulong> m_OnDownloadBinaryFileCompleteEvent;
        private WuWuFrameworkAction<string, Texture2D, ulong> m_OnDownloadTextureCompleteEvent;
        private WuWuFrameworkAction<string, AudioClip, ulong> m_OnDownloadAudioClipCompleteEvent;
        private WuWuFrameworkAction<string, VideoClip, ulong> m_OnDownloadVideoClipCompleteEvent;
        private WuWuFrameworkAction<string, AssetBundle, ulong> m_OnDownloadAssetBundleCompleteEvent;
        private WuWuFrameworkAction<string, ulong, ulong> m_OnDownloadProgressEvent;
        private WuWuFrameworkAction<string, string> m_OnDownloadErrorEvent;
        private UnityWebRequest m_WebRequest;
        private UnityWebRequestAsyncOperation m_WebRequestAsyncOperation;
        private ulong m_StartDownloadLength;

        public DownloadType downloadType { get; private set; }
        public string uri { get; private set; }
        public string version { get; private set; }
        public ulong downloadSize { get; private set; }
        public bool isDoing { get; private set; }
        public bool isDone { get; private set; }
        public bool isError { get; private set; }

        public event WuWuFrameworkAction<string, string, ulong> onDownloadTextCompleteEvent
        {
            add
            {
                m_OnDownloadTextCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadTextCompleteEvent -= value;
            }
        }

        public event WuWuFrameworkAction<string, string, ulong> onDownloadScriptCompleteEvent
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

        public event WuWuFrameworkAction<string, string, ulong> onDownloadBinaryFileCompleteEvent
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

        public event WuWuFrameworkAction<string, Texture2D, ulong> onDownloadTextureCompleteEvent
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

        public event WuWuFrameworkAction<string, AudioClip, ulong> onDownloadAudioClipCompleteEvent
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
        public event WuWuFrameworkAction<string, VideoClip, ulong> onDownloadVideoClipCompleteEvent
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

        public event WuWuFrameworkAction<string, AssetBundle, ulong> onDownloadAssetBundleCompleteEvent
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

        public event WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent
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

        public event WuWuFrameworkAction<string, string> onDownloadErrorEvent
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

        public static DownloadRequest Create(DownloadType downloadType, string uri, string version, ulong downloadSize)
        {
            DownloadRequest downloadRequest = ReferencePool.Acquire<DownloadRequest>();
            downloadRequest.downloadType = downloadType;
            downloadRequest.uri = uri;
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
            StopDownload();
            uri = null;
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
        }

        public void StartDownload()
        {
            if (isDoing || isDone || isError)
            {
                return;
            }

            isDoing = true;
            isDone = false;
            isError = false;

            m_WebRequest = CreateWebRequest();

            if (m_WebRequest == null)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_OnDownloadErrorEvent?.Invoke(uri, "请求失败，请检查链接是否正确");
                throw new WuWuFrameworkException("请求失败，请检查链接是否正确");
            }

            m_StartDownloadLength = 0;

            if (m_WebRequest.downloadHandler is DownloadHandlerFile downloadHandlerFile)
            {
                m_StartDownloadLength = downloadHandlerFile.startDownloadLength;
            }

            m_WebRequestAsyncOperation = m_WebRequest.SendWebRequest();
        }

        public void StopDownload()
        {
            isDoing = false;
            isDone = false;
            isError = false;
            m_WebRequest?.Dispose();
            m_WebRequest.downloadHandler?.Dispose();
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
                Log.LogInfo("当前进度：", (m_StartDownloadLength + m_WebRequest.downloadedBytes).ToString());
                m_OnDownloadProgressEvent?.Invoke(uri, m_StartDownloadLength + m_WebRequest.downloadedBytes, downloadSize);
                return;
            }

            if (m_WebRequest.result == UnityWebRequest.Result.ConnectionError || m_WebRequest.result == UnityWebRequest.Result.ProtocolError || m_WebRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                OnDownloadError(m_WebRequest.error);
            }
            else if (m_WebRequest.result == UnityWebRequest.Result.Success)
            {
                OnDownloadComplete(downloadType, m_WebRequest);
            }
        }

        private UnityWebRequest CreateWebRequest()
        {
            UnityWebRequest uwr = null;
            DownloadHandler downloadHandler = null;

            switch (downloadType)
            {
                case DownloadType.Text:
                    uwr = UnityWebRequest.Get(uri);
                    break;
                case DownloadType.AssetBundle:
                    uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    downloadHandler = new DownloadHandlerAssetBundle(uri, 0);
                    break;
                case DownloadType.BinaryFile:
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
            m_OnDownloadErrorEvent?.Invoke(uri, errorMessage);
            throw new WuWuFrameworkException("请求失败，请检查链接是否正确");
        }

        private void OnDownloadComplete(DownloadType downloadType, UnityWebRequest uwr)
        {
            switch (downloadType)
            {
                case DownloadType.Text:
                    OnTextDownloaded(uwr);
                    break;
                case DownloadType.AssetBundle:
                    OnAssetBundleDownloaded(uwr);
                    break;
                case DownloadType.BinaryFile:
                    OnBinaryFileDownloaded();
                    break;
                case DownloadType.Texture:
                    OnTextureDownloaded(uwr);
                    break;
                case DownloadType.AudioClip:
                    OnAudioClipDownloaded(uwr);
                    break;
                case DownloadType.VideoClip:
                    throw new WuWuFrameworkException("直接使用File类型");
                case DownloadType.Script:
                    OnScriptDownloaded(uwr);
                    break;
            }

            isDoing = false;
            isDone = true;
            isError = false;
        }

        private void OnTextDownloaded(UnityWebRequest uwr)
        {
            m_OnDownloadTextCompleteEvent?.Invoke(uri, uwr.downloadHandler.text, downloadSize);
        }

        private void OnBinaryFileDownloaded()
        {
            m_OnDownloadBinaryFileCompleteEvent?.Invoke(uri, version, downloadSize);
        }

        private void OnAssetBundleDownloaded(UnityWebRequest uwr)
        {
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
            m_OnDownloadAssetBundleCompleteEvent?.Invoke(uri, assetBundle, downloadSize);
        }

        private void OnScriptDownloaded(UnityWebRequest uwr)
        {
            m_OnDownloadScriptCompleteEvent?.Invoke(uri, uwr.downloadHandler.text, downloadSize);
        }

        private void OnTextureDownloaded(UnityWebRequest uwr)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
            m_OnDownloadTextureCompleteEvent?.Invoke(uri, texture, downloadSize);
        }

        private void OnAudioClipDownloaded(UnityWebRequest uwr)
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            m_OnDownloadAudioClipCompleteEvent?.Invoke(uri, audioClip, downloadSize);
        }
    }
}