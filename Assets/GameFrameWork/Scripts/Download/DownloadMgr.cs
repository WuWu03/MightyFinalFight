using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Download
{
    public enum DownloadType
    {
        File,
        Buffer,
        Script,
        AssetBundle,
        Texture,
        AudioClip,
        VideoClip,
    }

    public class DownloadMgr : BaseMgr<DownloadMgr>
    {
        protected override void OnAwake()
        {
            m_DownloadRequests = new();
        }

        protected override void OnFixedUpdate()
        {
            if (m_DownloadRequests != null && m_DownloadRequests.Count > 0)
            {
                for (int i = m_DownloadRequests.Count - 1; i > -1; i--)
                {
                    if (m_DownloadRequests[i].isDone || m_DownloadRequests[i].isError)
                    {
                        RemoveDownload(m_DownloadRequests[i]);
                    }
                }
            }

            if (m_DownloadRequests != null && m_DownloadRequests.Count > 0 && !m_DownloadRequests[0].isDoing)
            {
                m_DownloadRequests[0].StartDownload();
            }
        }

        protected override void OnShutDown()
        {
            RemoveAllDownload();

            for (int i = 0; i < m_DownloadRequests.Count; i++) 
            { 
                m_DownloadRequests[i].Release(); 
            }

            m_DownloadRequests.Clear();
        }

        protected override void OnDestory()
        {
            m_DownloadRequests = null;
        }

        public void StartDownload()
        {
            if (m_DownloadRequests != null && m_DownloadRequests.Count > 0 && !m_DownloadRequests[0].isDoing)
            {
                m_DownloadRequests[0].StartDownload();
            }
        }

        public void StopDownload()
        {
            if (m_DownloadRequests != null && m_DownloadRequests.Count > 0 && m_DownloadRequests[0].isDoing)
            {
                m_DownloadRequests[0].StopDownload();
            }
        }

        public void AddDownloadFile(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, ulong> onDownloadBinaryFileCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(this, DownloadType.File, uri, tag, version, downloadSize);
            downloadRequest.onDownloadBinaryFileCompleteEvent += onDownloadBinaryFileCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void AddDownloadScriptFile(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, string, ulong> onDownloadTextFileCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(this, DownloadType.Script, uri, tag, version, downloadSize);
            downloadRequest.onDownloadScriptCompleteEvent += onDownloadTextFileCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void AddDownloadAssetBundle(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, AssetBundle, ulong> onDownloadTextureCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(this, DownloadType.AssetBundle, uri, tag, version, downloadSize);
            downloadRequest.onDownloadAssetBundleCompleteEvent += onDownloadTextureCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }

        public void AddDownloadTexture(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, Texture2D, ulong> onDownloadTextureCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(this, DownloadType.Texture, uri, tag, version, downloadSize);
            downloadRequest.onDownloadTextureCompleteEvent += onDownloadTextureCompleteEvent;
            downloadRequest.onDownloadProgressEvent += onDownloadProgressEvent;
            downloadRequest.onDownloadErrorEvent += onDownloadErrorEvent;
            m_DownloadRequests.Add(downloadRequest);
        }


        public void AddDownloadAudioClip(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, AudioClip, ulong> onDownloadAudioClipCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent)
        {
            DownloadRequest downloadRequest = DownloadRequest.Create(this, DownloadType.AudioClip, uri, tag, version, downloadSize);
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

        private void RemoveDownload(DownloadRequest downloadRequest)
        {
            downloadRequest.StopDownload();
            downloadRequest.Release();
            m_DownloadRequests.Remove(downloadRequest);
        }

        private List<DownloadRequest> m_DownloadRequests = null;
    }
}