/*
 * @Desc: Title 模块 TitleView 界面视图
 * @Date: 2021-09-06 21:09:22
 * @Author: WuWu
 */

using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class TitleView : UIBaseView<TitleViewComponent, TitleViewSettings>
{
    protected override void OnOpen(object arg)
    {

    }

    protected override void OnShow(object arg)
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
                component.txtIntroTmp.DOKill(true);
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

    protected override void OnHide()
    {
        
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
            component.txtStart.Append("(START)");
            component.txtSettings.Append("(SELECT)");

        }
        else
        {
            component.txtStart.Append("(G)");
            component.txtSettings.Append("(H)");
        }
    }

    private void StartGame()
    {
        AudioMgr.instance.FadeBgm(0, 0, 1);
        LoadMgr.instance.DOFadeBlack(OnLoadFadeBlackComplete);
    }

    private void OnLoadFadeBlackComplete()
    {
        UIMgr.instance.Open<RoleSelectView>();
        CloseSelf();
    }

    private void TitleAnim()
    {
        m_CanStart = false;
        m_CanSkipOpening = false;
        component.imgCapcom.color = new Color(1, 1, 1, 0);
        component.txtDeveloper.color = new Color(1, 1, 1, 0);
        component.imgStar.color = new Color(1, 1, 0.3f, 0);
        component.imgLogoBG.fillAmount = 0f;
        component.imgRetro.fillAmount = 0f;

        component.imgLogoBG.gameObject.SetActiveSelf(false);
        component.imgRetro.gameObject.SetActiveSelf(false);
        component.imgLogo.gameObject.SetActiveSelf(false);
        component.imgStar.gameObject.SetActiveSelf(false);
        component.txtStart.gameObject.SetActiveSelf(false);
        component.txtSettings.gameObject.SetActiveSelf(false);
        component.txtDeveloper.gameObject.SetActiveSelf(false);
        component.imgCapcom.gameObject.SetActiveSelf(true);
        component.txtIntro.gameObject.SetActiveSelf(false);
        component.imgIntro1.gameObject.SetActiveSelf(false);
        component.imgIntro2.gameObject.SetActiveSelf(false);
        component.txtIntro.SetText(string.Empty);

        m_AnimSequence.Append(component.imgCapcom.DOFade(1, 2));
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.Append(component.imgCapcom.DOFade(0, 2));
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgCapcom.gameObject.SetActiveSelf(false);
            component.txtDeveloper.gameObject.SetActiveSelf(true);
        });
        m_AnimSequence.Append(component.txtDeveloper.DOFade(1, 2));
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.Append(component.txtDeveloper.DOFade(0, 2));
        m_AnimSequence.AppendCallback(() =>
        {
            component.txtDeveloper.gameObject.SetActiveSelf(false);
            component.txtIntro.gameObject.SetActiveSelf(true);
            m_CanSkipOpening = true;
        });
    }

    private void OpeningAnim()
    {
        DoOpeningText(0, 4);
        m_AnimSequence.AppendCallback(() =>
        {
            component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound/Phone.wav"));
        });
        m_AnimSequence.AppendInterval(8f);
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgIntro1.gameObject.SetActiveSelf(true);
            component.imgIntro1.color = new Color(1, 1, 1, 0);
            component.imgIntro1.DOFade(1, 1f).SetEase(Ease.Linear);
            component.txtIntro.SetText(string.Empty);
            component.txtIntroTmp.color = Color.white;
            AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmOpening), false);
        });
        m_AnimSequence.AppendInterval(1f);
        DoOpeningText(5, 6);
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgIntro1.DOFade(0, 1f).SetEase(Ease.Linear);
            component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
        });

        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgIntro1.gameObject.SetActiveSelf(false);
            component.txtIntroTmp.DOKill(true);
            component.txtIntro.SetText(string.Empty);
            component.txtIntroTmp.color = Color.white;
        });
        DoOpeningText(7, 7);
        m_AnimSequence.AppendInterval(2);
        m_AnimSequence.Append(component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear));
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgIntro2.gameObject.SetActiveSelf(true);
            component.imgIntro2.color = new Color(1, 1, 1, 0);
            component.imgIntro2.DOFade(1, 1f).SetEase(Ease.Linear);
            component.txtIntroTmp.DOKill(true);
            component.txtIntro.SetText(string.Empty);
            component.txtIntroTmp.color = Color.white;
        });
        m_AnimSequence.AppendInterval(1f);
        DoOpeningText(8, 11);
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgIntro2.color = Color.white;
            component.imgIntro2.DOFade(0, 1f).SetEase(Ease.Linear);
            component.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
        });

        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.AppendCallback(() =>
        {
            component.txtIntroTmp.DOKill(true);
            component.txtIntro.SetText(string.Empty);
            component.txtIntroTmp.color = Color.white;
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
            component.txtIntro.gameObject.SetActiveSelf(false);
            component.imgIntro1.gameObject.SetActiveSelf(false);
            component.imgIntro2.gameObject.SetActiveSelf(false);
            component.imgLogo.gameObject.SetActiveSelf(true);
            component.imgLogo.transform.localScale = Vector3.one * 3;
            component.imgLogo.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
        });
        m_AnimSequence.AppendInterval(0.45f);
        m_AnimSequence.AppendCallback(() =>
        {
            AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BicycleKick));
        });
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgLogoBG.gameObject.SetActiveSelf(true);
            component.imgRetro.gameObject.SetActiveSelf(true);
            component.imgLogoBG.DOFillAmount(1, 0.2f);
            component.imgRetro.DOFillAmount(1, 0.2f);
        });
        m_AnimSequence.AppendInterval(0.2f);
        m_AnimSequence.AppendCallback(() =>
        {
            component.imgStar.gameObject.SetActiveSelf(true);
            AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmTitle), false, 1, 0);
        });
        m_AnimSequence.AppendInterval(0.1f);
        m_AnimSequence.Append(component.imgStar.DOFade(1, 1f));
        m_AnimSequence.AppendCallback(() =>
        {
            component.txtStart.gameObject.SetActiveSelf(true);
            component.txtSettings.gameObject.SetActiveSelf(true);
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

                component.txtIntro.SetText(string.Empty);
                component.txtIntroTmp.DOText(content, 1.8f).SetEase(Ease.Linear).OnUpdate(() =>
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