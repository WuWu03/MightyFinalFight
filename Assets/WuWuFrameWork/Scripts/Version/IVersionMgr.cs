using WuWuFramework.Download;
using WuWuFramework.Event;

namespace WuWuFramework.Version
{
    /// <summary>
    /// 版本验证状态
    /// </summary>
    public enum VersionProcessState
    {
        /// <summary>
        /// 不进行版本验证
        /// </summary>
        DontCheckVersion,
        /// <summary>
        /// 版本验证中
        /// </summary>
        CheckVersion,
        /// <summary>
        /// 版本验证链接错误
        /// </summary>
        CheckVersionUriError,
        /// <summary>
        /// 下载记录资源版本号的文件发生错误
        /// </summary>
        CheckVersionFileError,
        /// <summary>
        /// 版本验证完成
        /// </summary>
        CheckVersionComplete,
        /// <summary>
        /// 版本解析中
        /// </summary>
        VersionAnalyze,
        /// <summary>
        /// 版本解析错误
        /// </summary>
        VersionAnalyzeError,
        /// <summary>
        /// 版本解析完成
        /// </summary>
        VersionAnalyzeComplete,
        /// <summary>
        /// 下载文件中
        /// </summary>
        DownloadFiles,
        /// <summary>
        /// 下载文件错误
        /// </summary>
        DownloadFilesError,
        /// <summary>
        /// 下载文件完成
        /// </summary>
        DownloadFilesComplete,
        /// <summary>
        /// 版本验证成功
        /// </summary>
        Success,
        /// <summary>
        /// 版本验证错误
        /// </summary>
        Error,
    }

    public interface IVersionMgr
    {
        /// <summary>
        /// 版本验证进度事件
        /// </summary>
        public event WuWuFrameworkAction<VersionProcessState, string, ulong, ulong> onVersionProcessStateChangedEvent;

        /// <summary>
        /// 注入DownloadMgr依赖
        /// </summary>
        /// <param name="downloadMgr"></param>
        public void SetDownloadMgr(IDownloadMgr downloadMgr);

        /// <summary>
        /// 设置版本验证链接
        /// </summary>
        /// <param name="uri"></param>
        public void SetCheckVersionUri(string uri);
    }
}
