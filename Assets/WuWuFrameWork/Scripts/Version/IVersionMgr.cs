using WuWuFramework.Download;
using WuWuFramework.Event;
using WuWuFramework.WebRequest;

namespace WuWuFramework.Version
{
    public enum VersionProcessState
    {
        DontCheckVersion,
        CheckVersion,
        CheckVersionUriError,
        CheckVersionFileError,
        CheckVersionComplete,
        VersionAnalyze,
        VersionAnalyzeError,
        VersionAnalyzeComplete,
        DownloadFiles,
        DownloadFilesError,
        DownloadFilesComplete,
        Success,
        Error,
    }

    public interface IVersionMgr
    {
        public event WuWuFrameworkAction<VersionProcessState, string, ulong, ulong> onVersionProcessStateChangedEvent;
        public void SetMgr(IDownloadMgr downloadMgr, IWebRequestMgr webRequestMgr);
        public void SetCheckVersionUri(string uri);
    }
}
