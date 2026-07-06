using UnityEngine;
using WuWuFramework.Event;

namespace WuWuFramework.Download
{
    public enum DownloadType : byte
    {
        Text,
        BinaryFile,
        Buffer,
        Script,
        AssetBundle,
        Texture,
        AudioClip,
        VideoClip,
    }

    public interface IDownloadMgr
    {
        /// <summary>
        /// 开始下载
        /// </summary>
        public void StartDownload();

        /// <summary>
        /// 停止下载
        /// </summary>
        public void StopDownload();

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
           WuWuFrameworkAction<string, string> onDownloadErrorEvent);

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
            WuWuFrameworkAction<string, string> onDownloadErrorEvent);

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
            WuWuFrameworkAction<string, string> onDownloadErrorEvent);


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
            WuWuFrameworkAction<string, string> onDownloadErrorEvent);

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
            WuWuFrameworkAction<string, string> onDownloadErrorEvent);

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
            WuWuFrameworkAction<string, string> onDownloadErrorEvent);

        /// <summary>
        /// 移除下载器
        /// </summary>
        /// <param name="uri"></param>
        public void RemoveDownload(string uri);

        /// <summary>
        /// 移除所有下载器
        /// </summary>
        public void RemoveAllDownload();
    }
}