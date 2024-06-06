/*******************************************************/
/**2021-9-6 21:9****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.UI;
using System;
using UnityEngine;

public class TitlePanel : BasePanel
{
	protected override Type componentType
	{
		get
		{
			return typeof(TitlePanelComponent);
		}
	}

    protected override Type settingsType
    {
        get
        {
            return typeof(TitlePanelSettings);
        }
    }

    protected override void OnInit(BasePanelComponent panelComponent, object[] param)
	{
		m_Component = panelComponent as TitlePanelComponent;
	}

	protected override void OnOpen()
	{
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

	}

	protected override void OnDestroy()
	{
	}

	private void StartGame()
	{
		LoadPanel loadPanel = UIMgr.instance.Open<LoadPanel>();
		loadPanel.DOFade(0f, 1f, 0.3f, 0.5f, () =>
		{
			UIMgr.instance.Open<RoleSelectPanel>();
			CloseSelf();
		});

		loadPanel.DOFade(1, 0, 0.3f, 0.1f, () =>
		{
			UIMgr.instance.Close<LoadPanel>();
		});
	}

	private void TitleAnim()
	{
		m_CanStart = false;
		m_Component.imgCapcom.color = new Color(1, 1, 1, 0);
		m_Component.txtDeveloper.color = new Color(1, 1, 1, 0);
		m_Component.imgStar.color = new Color(1, 1, 0.3f, 0);
		m_Component.imgLogoBG.fillAmount = 0f;
		m_Component.imgRetro.fillAmount = 0f;

		m_Component.imgLogoBG.gameObject.SetActive(false);
		m_Component.imgRetro.gameObject.SetActive(false);
		m_Component.imgLogo.gameObject.SetActive(false);
		m_Component.imgStar.gameObject.SetActive(false);
		m_Component.txtStart.gameObject.SetActive(false);
		m_Component.txtDeveloper.gameObject.SetActive(false);
		m_Component.imgCapcom.gameObject.SetActive(true);

		Sequence sequence = DOTween.Sequence();
		sequence.Append(m_Component.imgCapcom.DOFade(1, 2));
		sequence.AppendInterval(1f);
		sequence.Append(m_Component.imgCapcom.DOFade(0, 2));
		sequence.AppendCallback(() =>
		{
			m_Component.imgCapcom.gameObject.SetActive(false);
			m_Component.txtDeveloper.gameObject.SetActive(true);
		});
		sequence.Append(m_Component.txtDeveloper.DOFade(1, 2));
		sequence.AppendInterval(1f);
		sequence.Append(m_Component.txtDeveloper.DOFade(0, 2));
		sequence.AppendCallback(() =>
		{
			m_Component.txtDeveloper.gameObject.SetActive(false);
			m_Component.imgLogo.gameObject.SetActive(true);
			m_Component.imgLogo.transform.localScale = Vector3.one * 3;
		});
		sequence.Append(m_Component.imgLogo.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce));
		sequence.InsertCallback(10.2f, () =>
		{
			AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, SoundName.BicycleKick);
		});
		sequence.AppendCallback(() =>
		{
			m_Component.imgLogoBG.gameObject.SetActive(true);
			m_Component.imgRetro.gameObject.SetActive(true);
			m_Component.imgLogoBG.DOFillAmount(1, 0.2f);
			m_Component.imgRetro.DOFillAmount(1, 0.2f);
		});
		sequence.AppendInterval(0.2f);
		sequence.AppendCallback(() =>
		{
			m_Component.imgStar.gameObject.SetActive(true);
			AudioMgr.instance.PlayBGM(AssetPathDefine.AudioClipPath, SoundName.Bgm13Title, false);
		});
		sequence.AppendInterval(0.1f);
		sequence.Append(m_Component.imgStar.DOFade(1, 1f));
		sequence.AppendCallback(() =>
		{
			m_Component.txtStart.gameObject.SetActive(true);
			m_CanStart = true;
		});
	}

	private TitlePanelComponent m_Component = null;
	private bool m_CanStart = false;
}