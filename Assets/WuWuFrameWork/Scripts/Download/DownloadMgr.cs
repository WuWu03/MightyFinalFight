using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Event;

namespace WuWuFramework.Download
{
    public class DownloadMgr : WuWuFrameworkModule,IDownloadMgr
    {
        private readonly List<DownloadRequest> m_DownloadRequests;

        public DownloadMgr()
        {
            m_DownloadRequests = new();
            MonoBehaviourMgr.instance.updateEvent += Update;
        }
        
        public void StartDownload()
        {
            if (m_DownloadRequests is { Count: > 0 } && !m_DownloadRequests[0].isDoing)
            {
                m_DownloadRequests[0].StartDownload();
            }
        }

        public void StopDownload()
        {
            if (m_DownloadRequests is { Count: > 0 } && m_DownloadRequests[0].isDoing)
            {
                m_DownloadRequests[0].StopDownload();
            }
        }

        public void AddDownloadFile(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, ulong> onDownloadBinaryFileCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.File, uri, tag, version, downloadSize);
            downloadRequest.onDownloadBinaryFileCompleteEvent += onDownloadBinaryFileCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void AddDownloadScriptFile(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, string, ulong> onDownloadTextFileCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.Script, uri, tag, version, downloadSize);
            downloadRequest.onDownloadScriptCompleteEvent += onDownloadTextFileCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void AddDownloadAssetBundle(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, AssetBundle, ulong> onDownloadTextureCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.AssetBundle, uri, tag, version, downloadSize);
            downloadRequest.onDownloadAssetBundleCompleteEvent += onDownloadTextureCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void AddDownloadTexture(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, Texture2D, ulong> onDownloadTextureCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.Texture, uri, tag, version, downloadSize);
            downloadRequest.onDownloadTextureCompleteEvent += onDownloadTextureCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }


        public void AddDownloadAudioClip(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, AudioClip, ulong> onDownloadAudioClipCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(DownloadType.AudioClip, uri, tag, version, downloadSize);
            downloadRequest.onDownloadAudioClipCompleteEvent += onDownloadAudioClipCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void RemoveDownload(string uri, string tag)
        {
            if (m_DownloadRequests == null || m_DownloadRequests.Count == 0 || string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(tag))
            {
                return;
            }

            for (int i = m_DownloadRequests.Count - 1; i >= 0; i--)
            {
                if (m_DownloadRequests[i].uri == uri && m_DownloadRequests[i].tag == tag)
                {
                    RemoveDownload(m_DownloadRequests[i]);
                    break;
                }
            }
        }

        public void RemoveDownloadByUri(string uri)
        {
            if (m_DownloadRequests == null || m_DownloadRequests.Count == 0 || string.IsNullOrEmpty(uri))
            {
                return;
            }

            for (int i = m_DownloadRequests.Count - 1; i > -1; i--)
            {
                if (m_DownloadRequests[i].uri == uri)
                {
                    RemoveDownload(m_DownloadRequests[i]);
                }
            }
        }

        public void RemoveDownloadByTag(string tag)
        {
            if (m_DownloadRequests == null || m_DownloadRequests.Count == 0 || string.IsNullOrEmpty(tag))
            {
                return;
            }

            for (int i = m_DownloadRequests.Count - 1; i > -1; i--)
            {
                if (m_DownloadRequests[i].tag == tag)
                {
                    RemoveDownload(m_DownloadRequests[i]);
                }
            }
        }

        public void RemoveAllDownload()
        {
            if (m_DownloadRequests == null || m_DownloadRequests.Count == 0)
            {
                return;
            }

            for (int i = m_DownloadRequests.Count - 1; i >= 0; i--)
            {
                RemoveDownload(m_DownloadRequests[i]);
            }
        }

        public override void Shutdown()
        {
            RemoveAllDownload();

            foreach (var downloadRequest in m_DownloadRequests)
            {
                downloadRequest.Release();
            }

            m_DownloadRequests.Clear();
            MonoBehaviourMgr.instance.updateEvent -= Update;
        }

        private void RemoveDownload(DownloadRequest downloadRequest)
        {
            downloadRequest.StopDownload();
            downloadRequest.Release();
            m_DownloadRequests.Remove(downloadRequest);
        }

        private void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            if (m_DownloadRequests is { Count: > 0 })
            {
                DownloadRequest downloadRequest = m_DownloadRequests[0];

                if (downloadRequest.isDone || downloadRequest.isError)
                {
                    RemoveDownload(downloadRequest);
                }
                else if (!downloadRequest.isDoing)
                {
                    downloadRequest.StartDownload();
                }
            }
        }
    }
}