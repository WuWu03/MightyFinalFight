/*******************************************************/
/**2021-9-6 21:9****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class TitlePanel : BasePanel<TitlePanelComponent, TitlePanelSettings>
{
	protected override void OnInit(object arg)
	{

	}

	protected override void OnOpen()
	{
		InputMgr.instance.inputDeviceChangeEvent += OnInputDeviceChangeEvent;
		OnInputDeviceChangeEvent();
		TitleAnim();
    }

    protected override void OnUpdate()
	{
		if (!m_CanStart)
		{
			return;
		}

		if (InputMgr.instance.GetKeyDown(KeyType.Start))
		{
			m_CanStart = false;
			StartGame();
		}
	}

	protected override void OnClose()
	{
		InputMgr.instance.inputDeviceChangeEvent -= OnInputDeviceChangeEvent;
	}

	protected override void OnDestroy()
	{

	}

	private void OnInputDeviceChangeEvent()
	{
		if (InputMgr.instance.isJoystickInput)
		{
			m_Component.txtStart.Append("(START)");
			m_Component.txtSettings.Append("(SELECT)");

		}
		else
		{
			m_Component.txtStart.Append("(G)");
			m_Component.txtSettings.Append("(H)");
		}
	}

	private void StartGame()
	{
		AudioMgr.instance.FadeBgm(0, 0, 1);
		LoadPanelMgr.instance.DOFadeBlack(OnLoadFadeBlackComplete);
	}

	private void OnLoadFadeBlackComplete()
	{
		UIMgr.instance.Open(UINames.RoleSelectPanel);
		CloseSelf();
	}

	private void TitleAnim()
    {
        m_CanStart = false;
		m_Component.imgCapcom.color = new Color(1, 1, 1, 0);
		m_Component.txtDeveloper.color = new Color(1, 1, 1, 0);
		m_Component.imgStar.color = new Color(1, 1, 0.3f, 0);
		m_Component.imgLogoBG.fillAmount = 0f;
		m_Component.imgRetro.fillAmount = 0f;

        m_Component.imgLogoBG.gameObject.SetActiveSelf(false);
		m_Component.imgRetro.gameObject.SetActiveSelf(false);
		m_Component.imgLogo.gameObject.SetActiveSelf(false);
		m_Component.imgStar.gameObject.SetActiveSelf(false);
		m_Component.txtStart.gameObject.SetActiveSelf(false);
		m_Component.txtSettings.gameObject.SetActiveSelf(false);
		m_Component.txtDeveloper.gameObject.SetActiveSelf(false);
		m_Component.imgCapcom.gameObject.SetActiveSelf(true);

		Sequence sequence = DOTween.Sequence();
		sequence.Append(m_Component.imgCapcom.DOFade(1, 2));
		sequence.AppendInterval(1f);
		sequence.Append(m_Component.imgCapcom.DOFade(0, 2));
		sequence.AppendCallback(() =>
		{
			m_Component.imgCapcom.gameObject.SetActiveSelf(false);
			m_Component.txtDeveloper.gameObject.SetActiveSelf(true);
		});
		sequence.Append(m_Component.txtDeveloper.DOFade(1, 2));
		sequence.AppendInterval(1f);
		sequence.Append(m_Component.txtDeveloper.DOFade(0, 2));
		sequence.AppendCallback(() =>
		{
			m_Component.txtDeveloper.gameObject.SetActiveSelf(false);
			m_Component.imgLogo.gameObject.SetActiveSelf(true);
			m_Component.imgLogo.transform.localScale = Vector3.one * 3;
		});
		sequence.Append(m_Component.imgLogo.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce));
		sequence.InsertCallback(10.2f, () =>
		{
			AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BicycleKick));
		});
		sequence.AppendCallback(() =>
		{
			m_Component.imgLogoBG.gameObject.SetActiveSelf(true);
			m_Component.imgRetro.gameObject.SetActiveSelf(true);
			m_Component.imgLogoBG.DOFillAmount(1, 0.2f);
			m_Component.imgRetro.DOFillAmount(1, 0.2f);
		});
		sequence.AppendInterval(0.2f);
		sequence.AppendCallback(() =>
		{
			m_Component.imgStar.gameObject.SetActiveSelf(true);
			AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.Bgm13Title), false);
		});
		sequence.AppendInterval(0.1f);
		sequence.Append(m_Component.imgStar.DOFade(1, 1f));
		sequence.AppendCallback(() =>
		{
			m_Component.txtStart.gameObject.SetActiveSelf(true);
			m_Component.txtSettings.gameObject.SetActiveSelf(true);
			m_CanStart = true;
		});
	}

	private bool m_CanStart = false;
}