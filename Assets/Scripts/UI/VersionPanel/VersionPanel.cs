/*******************************************************/
/**2025-08-08 14:18****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using GameFrameWork.UI;
using GameFrameWork.Utils;
using GameFrameWork.Version;

public class VersionPanel : BasePanel<VersionPanelComponent, VersionPanelSettings>
{
	protected override void OnInit(object arg)
	{
        VersionMgr.instance.onVersionProcessStateChangedEvent += OnVersionProcessStateChanged;
    }

	protected override void OnOpen()
	{
        VersionMgr.instance.SetCheckVersionUri("http://localhost/StreamingAssets/");
    }

	protected override void OnUpdate()
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
                m_Component.txtVersion.SetLanguageTextKey("VersionPanelCheckVersion");
                break;
            case VersionProcessState.CheckVersionUriError:
                m_Component.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionError");
                break;
            case VersionProcessState.CheckVersionFileError:
                m_Component.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionError");
                break;
            case VersionProcessState.CheckVersionComplete:
                m_Component.txtVersion.SetLanguageTextKey("VersionPaneCheckVersionComplete");
                break;
            case VersionProcessState.VersionAnalyze:
                m_Component.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyze");
                break;
            case VersionProcessState.VersionAnalyzeError:
                m_Component.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyzeError");
                break;
            case VersionProcessState.VersionAnalyzeComplete:
                m_Component.txtVersion.SetLanguageTextKey("VersionPanelVersionAnalyzeComplete");
                break;
            case VersionProcessState.DownloadFiles:
                string downloadSizeText = StringUtil.FormatSize(downloadSize);
                string downloadFullSizeText = StringUtil.FormatSize(downloadFullSize);
                m_Component.txtVersion.SetLanguageTextKey("VersionPanelDownloadFiles");
                m_Component.txtVersion.SetLanguageTextParams(downloadSizeText, downloadFullSizeText);
                break;
            case VersionProcessState.DownloadFilesError:
                m_Component.txtVersion.SetLanguageTextKey("VersionPanelDownloadFilesError");
                break;
            case VersionProcessState.DownloadFilesComplete:
                break;
            case VersionProcessState.Success:
                break;
            case VersionProcessState.Error:
                m_Component.txtVersion.SetText(info);
                break;
        }
    }
}