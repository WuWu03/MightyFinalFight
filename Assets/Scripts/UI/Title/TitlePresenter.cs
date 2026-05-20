/*
 * @Desc: Title 模块 TitleView 界面视图
 * @Date: 2021-09-06 21:09:22
 * @Author: GQY
 */

using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Input;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class TitlePresenter : UIBasePresenter<TitleView, TitleViewSettings>
{
    private bool m_CanSkipOpening;
    private float m_TextTimer = -1f;
    private Sequence m_AnimSequence;
    private bool m_CanStart;

    protected override void OnOpen(object arg)
    {
    }

    protected override void OnShow(object arg)
    {
        GameEntry.inputMgr.inputDeviceChangeEvent += OnInputDeviceChangeEvent;
        OnInputDeviceChangeEvent();
        m_AnimSequence = DOTween.Sequence();
        TitleAnim();
        OpeningAnim();
    }

    protected override void OnUpdate()
    {
        if (m_CanSkipOpening)
        {
            if (GameEntry.inputMgr.GetKeyDown(KeyType.Start))
            {
                view.txtIntroTmp.DOKill(true);
                m_AnimSequence.Kill();
                m_AnimSequence = DOTween.Sequence();
                StartAnim();
                m_CanSkipOpening = false;
            }
        }

        if (m_CanStart)
        {
            if (GameEntry.inputMgr.GetKeyDown(KeyType.Start))
            {
                m_CanStart = false;
                StartGame();
            }
        }
    }

    protected override void OnHide()
    {
        m_AnimSequence.Kill();
        m_AnimSequence = null;
        GameEntry.inputMgr.inputDeviceChangeEvent -= OnInputDeviceChangeEvent;
    }

    protected override void OnClose()
    {
    }

    protected override void OnDestroy()
    {
    }

    private void OnInputDeviceChangeEvent()
    {
        Debug.Log("测试输入状态操了" + GameEntry.inputMgr.inputDeviceType);
        if (GameEntry.inputMgr.inputDeviceType == InputDeviceType.Joystick)
        {
            view.txtStart.Append("(START)");
            view.txtSettings.Append("(SELECT)");
        }
        else if (GameEntry.inputMgr.inputDeviceType == InputDeviceType.Keyboard)
        {
            view.txtStart.Append("(G)");
            view.txtSettings.Append("(H)");
        }
    }

    private void StartGame()
    {
        GameEntry.soundMgr.FadeBgm(0, 0, 1);
        LoadMgr.instance.DOFadeBlack(OnLoadFadeBlackComplete);
    }

    private void OnLoadFadeBlackComplete()
    {
        GameFrameWorkMgr.GetModule<IUIMgr>().Open<RoleSelectPresenter>();
        CloseSelf();
    }

    private void TitleAnim()
    {
        m_CanStart = false;
        m_CanSkipOpening = false;
        view.imgCapcom.color = new Color(1, 1, 1, 0);
        view.txtDeveloper.color = new Color(1, 1, 1, 0);
        view.imgStar.color = new Color(1, 1, 0.3f, 0);
        view.imgLogoBG.fillAmount = 0f;
        view.imgRetro.fillAmount = 0f;
        view.imgLogoBG.gameObject.SetActiveSelf(false);
        view.imgRetro.gameObject.SetActiveSelf(false);
        view.imgLogo.gameObject.SetActiveSelf(false);
        view.imgStar.gameObject.SetActiveSelf(false);
        view.txtStart.gameObject.SetActiveSelf(false);
        view.txtSettings.gameObject.SetActiveSelf(false);
        view.txtDeveloper.gameObject.SetActiveSelf(false);
        view.imgCapcom.gameObject.SetActiveSelf(true);
        view.txtIntro.gameObject.SetActiveSelf(false);
        view.imgIntro1.gameObject.SetActiveSelf(false);
        view.imgIntro2.gameObject.SetActiveSelf(false);
        view.txtIntro.SetText(string.Empty);
        m_AnimSequence.Append(view.imgCapcom.DOFade(1, 2));
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.Append(view.imgCapcom.DOFade(0, 2));
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgCapcom.gameObject.SetActiveSelf(false);
            view.txtDeveloper.gameObject.SetActiveSelf(true);
        });
        m_AnimSequence.Append(view.txtDeveloper.DOFade(1, 2));
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.Append(view.txtDeveloper.DOFade(0, 2));
        m_AnimSequence.AppendCallback(() =>
        {
            view.txtDeveloper.gameObject.SetActiveSelf(false);
            view.txtIntro.gameObject.SetActiveSelf(true);
            m_CanSkipOpening = true;
        });
    }

    private void OpeningAnim()
    {
        DoOpeningText(0, 4);
        m_AnimSequence.AppendCallback(() =>
        {
            view.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
            GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound/Phone.wav"));
        });
        m_AnimSequence.AppendInterval(8f);
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgIntro1.gameObject.SetActiveSelf(true);
            view.imgIntro1.color = new Color(1, 1, 1, 0);
            view.imgIntro1.DOFade(1, 1f).SetEase(Ease.Linear);
            view.txtIntro.SetText(string.Empty);
            view.txtIntroTmp.color = Color.white;
            GameEntry.soundMgr.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmOpening), false);
        });
        m_AnimSequence.AppendInterval(1f);
        DoOpeningText(5, 6);
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgIntro1.DOFade(0, 1f).SetEase(Ease.Linear);
            view.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
        });
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgIntro1.gameObject.SetActiveSelf(false);
            view.txtIntroTmp.DOKill(true);
            view.txtIntro.SetText(string.Empty);
            view.txtIntroTmp.color = Color.white;
        });
        DoOpeningText(7, 7);
        m_AnimSequence.AppendInterval(2);
        m_AnimSequence.Append(view.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear));
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgIntro2.gameObject.SetActiveSelf(true);
            view.imgIntro2.color = new Color(1, 1, 1, 0);
            view.imgIntro2.DOFade(1, 1f).SetEase(Ease.Linear);
            view.txtIntroTmp.DOKill(true);
            view.txtIntro.SetText(string.Empty);
            view.txtIntroTmp.color = Color.white;
        });
        m_AnimSequence.AppendInterval(1f);
        DoOpeningText(8, 11);
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgIntro2.color = Color.white;
            view.imgIntro2.DOFade(0, 1f).SetEase(Ease.Linear);
            view.txtIntroTmp.DOFade(0, 1f).SetEase(Ease.Linear);
        });
        m_AnimSequence.AppendInterval(1f);
        m_AnimSequence.AppendCallback(() =>
        {
            view.txtIntroTmp.DOKill(true);
            view.txtIntro.SetText(string.Empty);
            view.txtIntroTmp.color = Color.white;
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
            GameEntry.soundMgr.StopAllSes();
            view.txtIntro.gameObject.SetActiveSelf(false);
            view.imgIntro1.gameObject.SetActiveSelf(false);
            view.imgIntro2.gameObject.SetActiveSelf(false);
            view.imgLogo.gameObject.SetActiveSelf(true);
            view.imgLogo.transform.localScale = Vector3.one * 3;
            view.imgLogo.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);
        });
        m_AnimSequence.AppendInterval(0.45f);
        m_AnimSequence.AppendCallback(() => { GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BicycleKick)); });
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgLogoBG.gameObject.SetActiveSelf(true);
            view.imgRetro.gameObject.SetActiveSelf(true);
            view.imgLogoBG.DOFillAmount(1, 0.2f);
            view.imgRetro.DOFillAmount(1, 0.2f);
        });
        m_AnimSequence.AppendInterval(0.2f);
        m_AnimSequence.AppendCallback(() =>
        {
            view.imgStar.gameObject.SetActiveSelf(true);
            GameEntry.soundMgr.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, SoundName.BgmTitle), false);
        });
        m_AnimSequence.AppendInterval(0.1f);
        m_AnimSequence.Append(view.imgStar.DOFade(1, 1f));
        m_AnimSequence.AppendCallback(() =>
        {
            view.txtStart.gameObject.SetActiveSelf(true);
            view.txtSettings.gameObject.SetActiveSelf(true);
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
                string content = GameEntry.localizationMgr.GetLanguageText(key);
                view.txtIntro.SetText(string.Empty);
                view.txtIntroTmp.ClampTextWidth(content);
                view.txtIntroTmp.DOText(content, 1.8f).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    if (m_TextTimer < 0 || Time.time - m_TextTimer > 0.1f)
                    {
                        m_TextTimer = Time.time;
                        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, "Sound/Text.wav"));
                    }
                });
            });

            m_AnimSequence.AppendInterval(3.8f);
        }
    }
}