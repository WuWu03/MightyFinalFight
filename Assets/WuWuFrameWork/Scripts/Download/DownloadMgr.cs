using System;
using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Event;

namespace WuWuFramework.Download
{
    public class DownloadMgr : WuWuFrameworkModule, IDownloadMgr
    {
        private readonly List<DownloadRequest> m_DownloadRequests;
        private const int MAX_DOING_REQUEST_COUNT = 10;
        private int m_MaxDoingRequestCount = 1;
        private int m_CurrDoingRequestCount = 0;
        private bool m_CanAutoStart = true;

        public DownloadMgr()
        {
            m_DownloadRequests = new();
            MonoBehaviourMgr.instance.updateEvent += Update;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void StartDownload()
        {
            m_CanAutoStart = true;
            int maxRequestCount = Math.Min(m_DownloadRequests.Count, m_MaxDoingRequestCount);

            if (m_CurrDoingRequestCount >= maxRequestCount)
            {
                return;
            }

            foreach (DownloadRequest downloadRequest in m_DownloadRequests)
            {
                if (!downloadRequest.isDoing)
                {
                    downloadRequest.StartDownload();
                    m_CurrDoingRequestCount++;

                    if (m_CurrDoingRequestCount >= maxRequestCount)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 停止下载
        /// </summary>
        public void StopDownload()
        {
            foreach (DownloadRequest downloadRequest in m_DownloadRequests)
            {
                downloadRequest.StopDownload();
            }

            m_CanAutoStart = false;
            m_CurrDoingRequestCount = 0;
        }

        /// <summary>
        /// 下载文本
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadSize"></param>
        /// <param name="onDownloadTextCompleteEvent"></param>
        /// <param name="onDownloadProgressEvent"></param>
        /// <param name="onDownloadErrorEvent"></param>
        public void AddDownloadText(string uri, ulong downloadSize,
            WuWuFrameworkAction<string, string, ulong> onDownloadTextCompleteEvent,
            WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string> onDownloadErrorEvent)
        {
            if (!VerifyUri(uri))
            {
                return;
            }

            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.Text, uri, string.Empty, downloadSize);
            downloadRequest.onDownloadTextCompleteEvent += onDownloadTextCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        /// <summary>
        /// 下载二进制文件到本地
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="version"></param>
        /// <param name="downloadSize"></param>
        /// <param name="onDownloadBinaryFileCompleteEvent"></param>
        /// <param name="onDownloadProgressEvent"></param>
        /// <param name="onDownloadErrorEvent"></param>
        public void AddDownloadBinaryFile(string uri, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, ulong> onDownloadBinaryFileCompleteEvent,
            WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string> onDownloadErrorEvent)
        {
            if (!VerifyUri(uri))
            {
                return;
            }

            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.BinaryFile, uri, version, downloadSize);
            downloadRequest.onDownloadBinaryFileCompleteEvent += onDownloadBinaryFileCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        /// <summary>
        /// 下载C#脚本
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadSize"></param>
        /// <param name="onDownloadTextFileCompleteEvent"></param>
        /// <param name="onDownloadProgressEvent"></param>
        /// <param name="onDownloadErrorEvent"></param>
        public void AddDownloadScriptFile(string uri, ulong downloadSize,
            WuWuFrameworkAction<string, string, ulong> onDownloadTextFileCompleteEvent,
            WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string> onDownloadErrorEvent)
        {
            if (!VerifyUri(uri))
            {
                return;
            }

            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.Script, uri, string.Empty, downloadSize);
            downloadRequest.onDownloadScriptCompleteEvent += onDownloadTextFileCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }


        /// <summary>
        /// 下载AssetBundle
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadSize"></param>
        /// <param name="onDownloadTextureCompleteEvent"></param>
        /// <param name="onDownloadProgressEvent"></param>
        /// <param name="onDownloadErrorEvent"></param>
        public void AddDownloadAssetBundle(string uri, ulong downloadSize,
            WuWuFrameworkAction<string, AssetBundle, ulong> onDownloadTextureCompleteEvent,
            WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string> onDownloadErrorEvent)
        {
            if (!VerifyUri(uri))
            {
                return;
            }

            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.AssetBundle, uri, string.Empty, downloadSize);
            downloadRequest.onDownloadAssetBundleCompleteEvent += onDownloadTextureCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadSize"></param>
        /// <param name="onDownloadTextureCompleteEvent"></param>
        /// <param name="onDownloadProgressEvent"></param>
        /// <param name="onDownloadErrorEvent"></param>
        public void AddDownloadTexture(string uri, ulong downloadSize,
            WuWuFrameworkAction<string, Texture2D, ulong> onDownloadTextureCompleteEvent,
            WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string> onDownloadErrorEvent)
        {
            if (!VerifyUri(uri))
            {
                return;
            }

            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.Texture, uri, string.Empty, downloadSize);
            downloadRequest.onDownloadTextureCompleteEvent += onDownloadTextureCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }


        /// <summary>
        /// 下载音频
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="downloadSize"></param>
        /// <param name="onDownloadAudioClipCompleteEvent"></param>
        /// <param name="onDownloadProgressEvent"></param>
        /// <param name="onDownloadErrorEvent"></param>
        public void AddDownloadAudioClip(string uri, ulong downloadSize,
            WuWuFrameworkAction<string, AudioClip, ulong> onDownloadAudioClipCompleteEvent,
            WuWuFrameworkAction<string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string> onDownloadErrorEvent)
        {
            if (!VerifyUri(uri))
            {
                return;
            }

            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.AudioClip, uri, string.Empty, downloadSize);
            downloadRequest.onDownloadAudioClipCompleteEvent += onDownloadAudioClipCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        /// <summary>
        /// 移除下载器
        /// </summary>
        /// <param name="uri"></param>
        public void RemoveDownload(string uri)
        {
            if (m_DownloadRequests == null || m_DownloadRequests.Count == 0 || string.IsNullOrEmpty(uri))
            {
                return;
            }

            for (int i = m_DownloadRequests.Count - 1; i >= 0; i--)
            {
                DownloadRequest downloadRequest = m_DownloadRequests[i];

                if (downloadRequest.uri == uri)
                {
                    downloadRequest.Release();
                    m_DownloadRequests.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 移除所有下载器
        /// </summary>
        public void RemoveAllDownload()
        {
            if (m_DownloadRequests == null || m_DownloadRequests.Count == 0)
            {
                return;
            }

            foreach (DownloadRequest downloadRequest in m_DownloadRequests)
            {
                downloadRequest.Release();
            }

            m_DownloadRequests.Clear();
        }

        /// <summary>
        /// 设置同时进行的Web请求数量，最大不超过10个，默认1个
        /// </summary>
        /// <param name="maxRequsetCount"></param>
        public void SetMaxRequsetCount(int maxRequsetCount)
        {
            m_MaxDoingRequestCount = Math.Min(maxRequsetCount, MAX_DOING_REQUEST_COUNT);
        }

        /// <summary>
        /// 框架关闭时清理下载列表
        /// </summary>
        public override void Shutdown()
        {
            RemoveAllDownload();
            MonoBehaviourMgr.instance.updateEvent -= Update;
        }

        private bool VerifyUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new WuWuFrameworkException("无效的链接");
            }

            if (m_DownloadRequests is not { Count: > 0 })
            {
                return true;
            }

            foreach (DownloadRequest downloadRequest in m_DownloadRequests)
            {
                if (downloadRequest.uri == uri)
                {
                    throw new WuWuFrameworkException("重复的请求 :[" + uri + "]");
                }
            }

            return true;
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (m_DownloadRequests is not { Count: > 0 })
            {
                return;
            }

            for (int i = m_DownloadRequests.Count - 1; i >= 0; i--)
            {
                DownloadRequest downloadRequest = m_DownloadRequests[i];

                if (downloadRequest.isDone || downloadRequest.isError)
                {
                    m_CurrDoingRequestCount--;
                    downloadRequest.Release();
                    m_DownloadRequests.RemoveAt(i);
                }
                else
                {
                    downloadRequest.Update();
                }
            }

            if (!m_CanAutoStart)
            {
                return;
            }

            StartDownload();
        }
    }
}