using UnityEngine;
using WuWuFramework.Event;

namespace WuWuFramework.Download
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
            WuWuFrameworkAction<string, string, string, ulong> onDownloadBinaryFileCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadScriptFile(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, string, ulong> onDownloadTextFileCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadAssetBundle(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, AssetBundle, ulong> onDownloadTextureCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadTexture(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, Texture2D, ulong> onDownloadTextureCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent);
        public void AddDownloadAudioClip(string uri, string tag, string version, ulong downloadSize,
            WuWuFrameworkAction<string, string, string, AudioClip, ulong> onDownloadAudioClipCompleteEvent,
            WuWuFrameworkAction<string, string, string, ulong, ulong> onDownloadProgressEvent,
            WuWuFrameworkAction<string, string, string, string> onDownloadErrorEvent);
        public void RemoveDownload(string uri, string tag);
        public void RemoveDownloadByUri(string uri);
        public void RemoveDownloadByTag(string tag);
        public void RemoveAllDownload();
    }
}