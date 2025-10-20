using GameFrameWork;
using GameFrameWork.Utils;

public class PlaySeClip : BaseClip
{
    private string m_AssetPath = string.Empty;
    private float m_Volume = 1;
    
    public static PlaySeClip Create(string assetPath, float volum = 1)
    {
        PlaySeClip playSeClip = ReferencePool.Acquire<PlaySeClip>();
        playSeClip.m_AssetPath = assetPath;
        playSeClip.m_Volume = volum;
        return playSeClip;
    }

    protected override void OnClear()
    {
        m_AssetPath = string.Empty;
        m_Volume = 1;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, m_AssetPath), m_Volume);
        Complete();
    }

    protected override void OnResume()
    {

    }
}