using GameFrameWork.Utils;
using System.Collections;
using System.IO;
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

        public event GameFrameWorkAction<string, string, string, string, ulong> onDownloadTextFileCompleteEvent
        {
            add
            {
                m_OnDownloadTextFileCompleteEvent += value;
            }
            remove
            {
                m_OnDownloadTextFileCompleteEvent -= value;
            }
        }

        public event GameFrameWorkAction<string, string, string, byte[], ulong> onDownloadBinaryFileCompleteEvent
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

        public event GameFrameWorkAction<string, string, string, byte[], ulong, ulong> onDownloadProgressEvent
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
            m_OnDownloadTextFileCompleteEvent = null;
            m_OnDownloadBinaryFileCompleteEvent = null;
            m_OnDownloadTextureCompleteEvent = null;
            m_OnDownloadAudioClipCompleteEvent = null;
            m_OnDownloadVideoClipCompleteEvent = null;
            m_OnDownloadAssetBundleCompleteEvent = null;
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
            m_MonoBehaviour.StopCoroutine(DownloadCoroutine());
        }

        private IEnumerator DownloadCoroutine()
        {
            string fileName = Path.GetFileNameWithoutExtension(uri);
            string downlaodVersionFilePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, fileName, ".downloadversion");
            string downloadTempFilePath = PathUtil.FormatPath(PathUtil.runTimeAssetsPath, fileName, ".downloadtemp");
            long startDownloadBytes = 0;

            if (!string.IsNullOrEmpty(this.version))
            {
                if (File.Exists(downlaodVersionFilePath))
                {
                    string version = File.ReadAllText(downlaodVersionFilePath);
                    if (this.version == version)
                    {
                        startDownloadBytes = GetDownloadBytesLength(downloadTempFilePath);
                    }
                    else
                    {
                        FileUtil.CreateTextFile(downlaodVersionFilePath, this.version);
                        FileUtil.DeleteFile(downloadTempFilePath);
                    }
                }
                else
                {
                    FileUtil.CreateTextFile(downlaodVersionFilePath, this.version);
                    FileUtil.DeleteFile(downloadTempFilePath);
                }
            }
            else
            {
                FileUtil.DeleteFile(downlaodVersionFilePath);
                startDownloadBytes = GetDownloadBytesLength(downloadTempFilePath);
            }

            UnityWebRequest uwr = CreateWebRequest(uri, startDownloadBytes);

            if (uwr == null)
            {
                isDoing = false;
                isDone = false;
                isError = true;
                m_OnDownloadErrorEvent?.Invoke(uri, tag, version, "请求失败，请检查链接是否正确");
                yield break;
            }

            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = uwr.SendWebRequest();

            while (!unityWebRequestAsyncOperation.isDone)
            {
                try
                {
                    Log.LogInfo("当前进度：", uwr.downloadedBytes.ToString());
                    File.WriteAllBytes(downloadTempFilePath, uwr.downloadHandler.data);
                    m_OnDownloadProgressEvent?.Invoke(uri, tag, version, uwr.downloadHandler.data, uwr.downloadedBytes, downloadSize);
                }
                catch
                {

                }

                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                OnDownloadError(uwr.error);
            }
            else if (uwr.result == UnityWebRequest.Result.Success)
            {
                OnDownloadComplete(downloadType, uwr);
                FileUtil.DeleteFile(downlaodVersionFilePath);
                FileUtil.DeleteFile(downloadTempFilePath);
            }
        }

        private long GetDownloadBytesLength(string downloadTempFilePath)
        {
            if (File.Exists(downloadTempFilePath))
            {
                byte[] downloadBytes = File.ReadAllBytes(downloadTempFilePath);
                long startDownloadBytes = downloadBytes.LongLength;
                m_OnDownloadProgressEvent?.Invoke(uri, tag, version, downloadBytes, (ulong)startDownloadBytes, downloadSize);
                return downloadBytes.LongLength;
            }

            FileUtil.CreateBinaryFile(downloadTempFilePath, null);
            return 0;
        }

        private UnityWebRequest CreateWebRequest(string uri, long startDownloadBytes)
        {
            UnityWebRequest uwr = null;
            DownloadHandler downloadHandler = null;

            switch (downloadType)
            {
                case DownloadType.AssetBundle:
                    uwr = UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    downloadHandler = new DownloadHandlerAssetBundle(uri, 0);
                    break;
                case DownloadType.TextFile:
                    uwr = UnityWebRequest.Get(uri);
                    break;
                case DownloadType.BinaryFile:
                    uwr = UnityWebRequest.Get(uri);
                    downloadHandler = new DownloadHandlerBuffer();
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
            }

            uwr.SetRequestHeader("Range", "bytes=" + startDownloadBytes + "-");

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
                case DownloadType.TextFile:
                    OnTextFileDownloaded(uwr);
                    break;
                case DownloadType.BinaryFile:
                    OnBinaryFileDownloaded(uwr);
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
                    OnTextFileDownloaded(uwr);
                    break;
            }
        }

        private void OnAssetBundleDownloaded(UnityWebRequest uwr)
        {
            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
            m_OnDownloadAssetBundleCompleteEvent?.Invoke(uri, tag, version, assetBundle, downloadSize);
        }

        private void OnTextFileDownloaded(UnityWebRequest uwr)
        {
            m_OnDownloadTextFileCompleteEvent?.Invoke(uri, tag, version, uwr.downloadHandler.text, downloadSize);
        }

        private void OnBinaryFileDownloaded(UnityWebRequest uwr)
        {
            m_OnDownloadBinaryFileCompleteEvent?.Invoke(uri, tag, version, uwr.downloadHandler.data, downloadSize);
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

        private GameFrameWorkAction<string, string, string, string, ulong> m_OnDownloadTextFileCompleteEvent;
        private GameFrameWorkAction<string, string, string, byte[], ulong> m_OnDownloadBinaryFileCompleteEvent;
        private GameFrameWorkAction<string, string, string, Texture2D, ulong> m_OnDownloadTextureCompleteEvent;
        private GameFrameWorkAction<string, string, string, AudioClip, ulong> m_OnDownloadAudioClipCompleteEvent;
        private GameFrameWorkAction<string, string, string, VideoClip, ulong> m_OnDownloadVideoClipCompleteEvent;
        private GameFrameWorkAction<string, string, string, AssetBundle, ulong> m_OnDownloadAssetBundleCompleteEvent;
        private GameFrameWorkAction<string, string, string, byte[], ulong, ulong> m_OnDownloadProgressEvent;
        private GameFrameWorkAction<string, string, string, string> m_OnDownloadErrorEvent;
        private MonoBehaviour m_MonoBehaviour = null;
    }
}