/*
 * @Desc: Version 模块 VersionView 视图展示器
 * @Date: 2025-08-08 14:18:49
 * @Author: WuWu
 */

using WuWuFramework.UI;
using WuWuFramework.Utils;
using WuWuFramework.Version;

public class VersionViewPresenter : UIBaseViewPresenter<VersionView>
{
    protected override void OnOpen(object arg)
    {
        GameEntry.versionMgr.onVersionProcessStateChangedEvent += OnVersionProcessStateChanged;
    }

    protected override void OnShow(object arg)
    {
        GameEntry.versionMgr.SetCheckVersionUri("http://localhost/StreamingAssets/");
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {

    }

    protected override void OnClose()
    {
        GameEntry.versionMgr.onVersionProcessStateChangedEvent -= OnVersionProcessStateChanged;
    }

    protected override void OnDestroy()
    {

    }

    private void OnVersionProcessStateChanged(VersionProcessState state, string info, ulong downloadSize, ulong downloadFullSize)
    {
        switch (state)
        {
            case VersionProcessState.DontCheckVersion:
                break;
            case VersionProcessState.CheckVersion:
                view.txtVersion.SetLanguageTextKey("VersionPanelCheckVersion");
                break;
            case VersionProcessState.CheckVersionUriError:
                view.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionError");
                break;
            case VersionProcessState.CheckVersionFileError:
                view.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionError");
                break;
            case VersionProcessState.CheckVersionComplete:
                view.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionComplete");
                break;
            case VersionProcessState.VersionAnalyze:
                view.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyze");
                break;
            case VersionProcessState.VersionAnalyzeError:
                view.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyzeError");
                break;
            case VersionProcessState.VersionAnalyzeComplete:
                view.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyzeComplete");
                break;
            case VersionProcessState.DownloadFiles:
                string downloadSizeText = StringUtil.FormatFileSize(downloadSize);
                string downloadFullSizeText = StringUtil.FormatFileSize(downloadFullSize);
                view.txtVersion.SetLanguageTextKey("VersionPanelDownloadFiles");
                view.txtVersion.SetLanguageTextParams(downloadSizeText, downloadFullSizeText);
                break;
            case VersionProcessState.DownloadFilesError:
                view.txtVersion.SetLanguageTextKey("VersionPanelDownloadFilesError");
                break;
            case VersionProcessState.DownloadFilesComplete:
                break;
            case VersionProcessState.Success:
                break;
            case VersionProcessState.Error:
                view.txtVersion.SetText(info);
                break;
        }
    }
}