using WuWuFramework.Event;
using WuWuFramework.Pool;
using UnityEngine;

namespace WuWuFramework.Sound
{
    public interface ISoundMgr
    {
        public bool isBgmComplete { get; }
        public event WuWuFrameworkAction onBgmFadeCompleteEvent;
        public void SetResourcePoolMgr(IResourcePoolMgr resourcePoolMgr, Transform root);
        public void PlaySe(string sePath, float volume = 1);
        public void StopAllSes();
        public void SetSePlaySpeed(float speed);
        public void PlayBgmGroup(BgmInfo[] bgmGroup, bool isForcePlay = false);
        public void PlayBgm(string assetPath, bool isLoop, float volume = 1, float lerpTime = 0, bool isForcePlay = false);
        public void StopBgm(bool isForceStop = false);
        public void PauseBgm();
        public void ResumeBgm();
        public void FadeBgm(float endValue, float delay, float duration);
        public bool IsBgmPlaying(string assetPath);
        public void SetBgmSpeed(float speed);
        public void ReleaseSeAudioSources();
    }
}