using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Utils;

public class PlayBgmClip : BaseClip
{
    public static PlayBgmClip Create(string assetPath, bool isLoop, float volume, float lerpTime, bool isForcePlay)
    {
        PlayBgmClip playBgmStory = ReferencePool.Acquire<PlayBgmClip>();
        playBgmStory.m_AssetPath = assetPath;
        playBgmStory.m_IsLoop = isLoop;
        playBgmStory.m_Volume = volume;
        playBgmStory.m_LerpTime = lerpTime;
        playBgmStory.m_IsForcePlay = isForcePlay;
        return playBgmStory;
    }

    protected override void OnClear()
    {
        m_AssetPath = string.Empty;
        m_IsLoop = false;
        m_Volume = 1;
        m_LerpTime = 0;
        m_IsForcePlay = false;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        AudioMgr.instance.PlayBgm(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, m_AssetPath), m_IsLoop, m_Volume, m_LerpTime, m_IsForcePlay);
        Complete();
    }

    protected override void OnResume()
    {

    }

    private string m_AssetPath = string.Empty;
    private bool m_IsLoop = false;
    private float m_Volume = 1;
    private float m_LerpTime = 0;
    private bool m_IsForcePlay = false;
}
