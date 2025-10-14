using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Utils;

public class PlaySeClip : BaseClip
{
    public static PlaySeClip Create(string assetPath, float volum = 1)
    {
        PlaySeClip playSeClip = ReferencePool.Acquire<PlaySeClip>();
        playSeClip.m_AssetPath = assetPath;
        playSeClip.m_Volum = volum;
        return playSeClip;
    }

    protected override void OnClear()
    {
        m_AssetPath = string.Empty;
        m_Volum = 1;
    }

    protected override void OnPause()
    {

    }

    protected override void OnPlay()
    {
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, m_AssetPath), m_Volum);
        Complete();
    }

    protected override void OnResume()
    {

    }

    private string m_AssetPath = string.Empty;
    private float m_Volum = 1;
}