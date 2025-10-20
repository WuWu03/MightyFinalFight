using GameFrameWork.Download;
using GameFrameWork.Event;
using GameFrameWork.WebRequest;

namespace GameFrameWork.Version
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
        public event GameFrameWorkAction<VersionProcessState, string, ulong, ulong> onVersionProcessStateChangedEvent;
        public void SetMgr(IDownloadMgr downloadMgr, IWebRequestMgr webRequestMgr);
        public void SetCheckVersionUri(string uri);
    }
}
