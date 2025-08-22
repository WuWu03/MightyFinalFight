/*******************************************************/
/**2021-9-6 21:9****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.Localization;
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
        m_AnimSequence = DOTween.Sequence();
        TitleAnim();
        OpeningAnim();
    }

    protected override void OnUpdate()
    {
        if (m_CanSkipOpening)
        {
            if (InputMgr.instance.GetKeyDown(KeyType.Start))
            {
                m_Component.txtIntroTmp.DOKill(true);
                m_AnimSequence.Kill();
                m_AnimSequence = DOTween.Sequence();
                StartAnim();
                m_CanSkipOpening = false;
            }
        }

        if (m_CanStart)
        {
            if (InputMgr.instance.GetKeyDown(KeyType.Start))
            {
                m_CanStart = false;
                StartGame();
            }
        }
    }

    protected override void OnClose()
    {
        m_AnimSequence.Kill(false);
        m_AnimSequence = null;
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
        m_CanSkipOpening = false;
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
        m_Component.txtIntro.gameObject.SetActiveSelf(false);
        m_Component.imgIntro1.gameObject.SetActiveSelf(false);
        m_Component.imgIntro2.gameObject.SetActiveSelf(false);
        m_Component.txtIntro.SetText(string.Empty);

        m_AnimSequence.Append(m_Component.imgCapcom.DOFade(1, 2));
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.Append(m_Component.imgCapcom.DOFade(0, 2));
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgCapcom.gameObject.SetActiveSelf(false);
            m_Component.txtDeveloper.gameObject.SetActiveSelf(true);
        });
        m_AnimSequence.Append(m_Component.txtDeveloper.DOFade(1, 2));
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.Append(m_Component.txtDeveloper.DOFade(0, 2));
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.txtDeveloper.gameObject.SetActiveSelf(false);
            m_Component.txtIntro.gameObject.SetActiveSelf(true);
            m_CanSkipOpening = true;
        });
    }

    private void OpeningAnim()
    {
        DoOpeningText(0, 4);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound/Phone.wav"));
        });
        m_AnimSequence.AppendInterval(8f);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgIntro1.gameObject.SetActiveSelf(true);
            m_Component.imgIntro1.color = new Color(1, 1, 1, 0);
            m_Component.imgIntro1.DOFade(1, 1f).SetEase(Ease.Linear);
            m_Component.txtIntro.SetText(string.Empty);
            m_Component.txtIntroTmp.color = Color.white;
            AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmOpening), false);
        });
        m_AnimSequence.AppendInterval(1f);
        DoOpeningText(5, 6);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgIntro1.DOFade(0, 1f).SetEase(Ease.Linear);
            m_Component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
        });

        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgIntro1.gameObject.SetActiveSelf(false);
            m_Component.txtIntroTmp.DOKill(true);
            m_Component.txtIntro.SetText(string.Empty);
            m_Component.txtIntroTmp.color = Color.white;
        });
        DoOpeningText(7, 7);
        m_AnimSequence.AppendInterval(2);
        m_AnimSequence.Append(m_Component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear));
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgIntro2.gameObject.SetActiveSelf(true);
            m_Component.imgIntro2.color = new Color(1, 1, 1, 0);
            m_Component.imgIntro2.DOFade(1, 1f).SetEase(Ease.Linear);
            m_Component.txtIntroTmp.DOKill(true);
            m_Component.txtIntro.SetText(string.Empty);
            m_Component.txtIntroTmp.color = Color.white;
        });
        m_AnimSequence.AppendInterval(1f);
        DoOpeningText(8, 11);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgIntro2.color = Color.white;
            m_Component.imgIntro2.DOFade(0, 1f).SetEase(Ease.Linear);
            m_Component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
        });

        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.txtIntroTmp.DOKill(true);
            m_Component.txtIntro.SetText(string.Empty);
            m_Component.txtIntroTmp.color = Color.white;
        });

        DoOpeningText(12, 12);
        m_AnimSequence.AppendInterval(0.4f);
        StartAnim();
    }
       
    private void StartAnim()
    {
        m_AnimSequence.AppendCallback(() =>
        {
            m_CanSkipOpening = false;
            AudioMgr.instance.StopAllSe();
            m_Component.txtIntro.gameObject.SetActiveSelf(false);
            m_Component.imgIntro1.gameObject.SetActiveSelf(false);
            m_Component.imgIntro2.gameObject.SetActiveSelf(false);
            m_Component.imgLogo.gameObject.SetActiveSelf(true);
            m_Component.imgLogo.transform.localScale = Vector3.one * 3;
            m_Component.imgLogo.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
        });
        m_AnimSequence.AppendInterval(0.45f);
        m_AnimSequence.AppendCallback(() =>
        {
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BicycleKick));
        });
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgLogoBG.gameObject.SetActiveSelf(true);
            m_Component.imgRetro.gameObject.SetActiveSelf(true);
            m_Component.imgLogoBG.DOFillAmount(1, 0.2f);
            m_Component.imgRetro.DOFillAmount(1, 0.2f);
        });
        m_AnimSequence.AppendInterval(0.2f);
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.imgStar.gameObject.SetActiveSelf(true);
            AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmTitle), false, 1, 0);
        });
        m_AnimSequence.AppendInterval(0.1f);
        m_AnimSequence.Append(m_Component.imgStar.DOFade(1, 1f));
        m_AnimSequence.AppendCallback(() =>
        {
            m_Component.txtStart.gameObject.SetActiveSelf(true);
            m_Component.txtSettings.gameObject.SetActiveSelf(true);
            m_CanStart = true;
        });
    }

    private void DoOpeningText(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            int storyIndex = i + 1;
            m_AnimSequence.AppendCallback(() =>
            {
                string key = StringUtil.Append("TitlePanelStory", storyIndex.ToString());
                string content = LocalizationMgr.instance.GetLanguageText(key);

                m_Component.txtIntro.SetText(string.Empty);
                m_Component.txtIntroTmp.DOText(content, 1.8f).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    if (m_TextTimer < 0 || Time.time - m_TextTimer > 0.1f)
                    {
                        m_TextTimer = Time.time;
                        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound/Text.wav"));
                    }
                });
            });

            m_AnimSequence.AppendInterval(3.8f);
        }
    }

    private bool m_CanSkipOpening = false;
    private float m_TextTimer = -1f;
    private Sequence m_AnimSequence = null;
    private bool m_CanStart = false;
}