/*******************************************************/
/**2025-08-08 14:18****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using GameFrameWork.UI;
using GameFrameWork.Utils;
using GameFrameWork.Version;

public class VersionView : UIBaseView<VersionComponent, VersionSettings>
{
	protected override void OnOpen(object arg)
	{
        VersionMgr.instance.onVersionProcessStateChangedEvent += OnVersionProcessStateChanged;
    }

	protected override void OnShow(object arg)
	{
        VersionMgr.instance.SetCheckVersionUri("http://localhost/StreamingAssets/");
    }

	protected override void OnUpdate()
	{
        
	}

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
	{
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
                component.txtVersion.SetLanguageTextKey("VersionPanelCheckVersion");
                break;
            case VersionProcessState.CheckVersionUriError:
                component.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionError");
                break;
            case VersionProcessState.CheckVersionFileError:
                component.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionError");
                break;
            case VersionProcessState.CheckVersionComplete:
                component.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionComplete");
                break;
            case VersionProcessState.VersionAnalyze:
                component.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyze");
                break;
            case VersionProcessState.VersionAnalyzeError:
                component.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyzeError");
                break;
            case VersionProcessState.VersionAnalyzeComplete:
                component.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyzeComplete");
                break;
            case VersionProcessState.DownloadFiles:
                string downloadSizeText = StringUtil.FormatFileSize(downloadSize);
                string downloadFullSizeText = StringUtil.FormatFileSize(downloadFullSize);
                component.txtVersion.SetLanguageTextKey("VersionPanelDownloadFiles");
                component.txtVersion.SetLanguageTextParams(downloadSizeText, downloadFullSizeText);
                break;
            case VersionProcessState.DownloadFilesError:
                component.txtVersion.SetLanguageTextKey("VersionPanelDownloadFilesError");
                break;
            case VersionProcessState.DownloadFilesComplete:
                break;
            case VersionProcessState.Success:
                break;
            case VersionProcessState.Error:
                component.txtVersion.SetText(info);
                break;
        }
    }
}