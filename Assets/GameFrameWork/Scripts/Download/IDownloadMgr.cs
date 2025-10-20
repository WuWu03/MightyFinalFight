using System.Collections.Generic;
using GameFrameWork.Event;
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

    public interface IDownloadMgr
    {
        public void StartDownload();
        public void StopDownload();
        public void AddDownloadFile(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, ulong> onDownloadBinaryFileCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadScriptFile(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, string, ulong> onDownloadTextFileCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadAssetBundle(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, AssetBundle, ulong> onDownloadTextureCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadTexture(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, Texture2D, ulong> onDownloadTextureCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadAudioClip(string uri, string tag, string version, ulong downloadSize,
            GameFrameWorkAction<string, string, string, AudioClip, ulong> onDownloadAudioClipCompleteEvent,
            GameFrameWorkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            GameFrameWorkAction<string, string, string, string> onDownloadErrorEvent);
        public void RemoveDownload(string uri, string tag);
        public void RemoveDownloadByUri(string uri);
        public void RemoveDownloadByTag(string tag);
        public void RemoveAllDownload();
    }
}