/*******************************************************/
/**2021-9-6 21:9****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Sound;
using GameFrameWork.Input;

public class TitlePanel : BasePanel
{
	public override string PanelName { get { return "TitlePanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Destroy; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new TitlePanelComponent(UIRefRoot);
	}

	protected override void OnOpen()
	{
		TitleAnim();
	}

	protected override void OnUpdate()
	{
		if(!m_CanStart)
        {
			return;
        }

		if (InputMgr.Ins.GetKeyDown(KeyType.Start, true))
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
		UIMgr.Ins.Open<LoadPanel>().DOFade(0, 1, 0.3f, 0.2f, () =>
		{
			UIMgr.Ins.Close<LoadPanel>();
			UIMgr.Ins.Open<RoleSelectPanel>();
			InnerClose();
		});
	}

	private void TitleAnim()
    {
		m_CanStart = false;
		m_Component.ImgCapcom.color = new Color(1, 1, 1, 0);
		m_Component.TxtDeveloper.color = new Color(1, 1, 1, 0);
		m_Component.ImgLogoBG.fillAmount = 0f;
		m_Component.ImgRetro.fillAmount = 0f;

		m_Component.ImgLogoBG.gameObject.SetActive(false);
		m_Component.ImgRetro.gameObject.SetActive(false);
		m_Component.ImgLogo.gameObject.SetActive(false);
		m_Component.TxtStart.gameObject.SetActive(false);
		m_Component.TxtDeveloper.gameObject.SetActive(false);
		m_Component.ImgCapcom.gameObject.SetActive(true);

		Sequence sequence = DOTween.Sequence();
		sequence.Append(m_Component.ImgCapcom.DOFade(1, 2));
		sequence.AppendInterval(1f);
		sequence.Append(m_Component.ImgCapcom.DOFade(0, 2));
		sequence.AppendCallback(() =>
		{
			m_Component.ImgCapcom.gameObject.SetActive(false);
			m_Component.TxtDeveloper.gameObject.SetActive(true);
		});
		sequence.Append(m_Component.TxtDeveloper.DOFade(1, 2));
		sequence.AppendInterval(1f);
		sequence.Append(m_Component.TxtDeveloper.DOFade(0, 2));
		sequence.AppendCallback(() =>
		{
			m_Component.TxtDeveloper.gameObject.SetActive(false);
			m_Component.ImgLogo.gameObject.SetActive(true);
			m_Component.ImgLogo.transform.localScale = Vector3.one * 3;
		});
		sequence.Append(m_Component.ImgLogo.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce));
		sequence.InsertCallback(10.2f, () =>
		{
			SoundMgr.Ins.PlaySound(ResDefine.AUDIO_CLIP_PATH, "Sound/BicycleKick");
		});
		sequence.AppendCallback(() =>
		{
			m_Component.ImgLogoBG.gameObject.SetActive(true);
			m_Component.ImgRetro.gameObject.SetActive(true);
			m_Component.ImgLogoBG.DOFillAmount(1, 0.2f);
			m_Component.ImgRetro.DOFillAmount(1, 0.2f);
		});
		sequence.AppendInterval(0.2f);
		sequence.AppendCallback(() =>
		{
			m_Component.TxtStart.gameObject.SetActive(true);
			SoundMgr.Ins.PlayBGM(ResDefine.AUDIO_CLIP_PATH, "BGM/bgm13Title", false);
			m_CanStart = true;
		});
	}

	private TitlePanelComponent m_Component = null;
	private bool m_CanStart = false;
}