using GameFrameWork;
using GameFrameWork.Audio;
using UnityEngine;

public class PlayBgmStory : BaseStory
{
    public static PlayBgmStory Create(string assetPath, bool isLoop, float volum, float lerpTime, bool isForcePlay)
    {
        PlayBgmStory playBgmStory = ReferencePool.Acquire<PlayBgmStory>();
        playBgmStory.m_AssetPath = assetPath;
        playBgmStory.m_IsLoop = isLoop;
        playBgmStory.m_Volum = volum;
        playBgmStory.m_LerpTime = lerpTime;
        playBgmStory.m_IsForcePlay = isForcePlay;
        return playBgmStory;
    }

    public override bool IsStoryComplete()
    {
        return isPlaying;
    }

    protected override void OnClear()
    {
        m_AssetPath = string.Empty;
        m_IsLoop = false;
        m_Volum = 1;
        m_LerpTime = 0;
        m_IsForcePlay = false;
    }

    protected override void OnPauseStory()
    {

    }

    protected override void OnPlayStory()
    {
        AudioMgr.instance.PlayBgm(m_AssetPath, m_IsLoop, m_Volum, m_LerpTime, m_IsForcePlay);
    }

    protected override void OnResumeStory()
    {

    }

    private string m_AssetPath = string.Empty;
    private bool m_IsLoop = false;
    private float m_Volum = 1;
    private float m_LerpTime = 0;
    private bool m_IsForcePlay = false;
}
