using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using WuWuFramework.Event;
using WuWuFramework.Pool;
using UnityObject = UnityEngine.Object;

namespace WuWuFramework.Sound
{
    public class SoundMgr : WuWuFrameworkModule, ISoundMgr
    {
        private readonly Queue<BgmInfo> m_WaitToPlayBgms;
        private readonly List<SeInfo> m_PlayingSes;
        private readonly Queue<SeInfo> m_UnUsedSes;
        private bool m_IsBgmPause;
        private AudioSource m_BgmAudioSource;
        private BgmInfo m_PlayingBgm;
        private GameObject m_Root;
        private IResourcePoolMgr m_ResourcePoolMgr;
        private event WuWuFrameworkAction m_OnBgmFadeCompleteEvent;

        public SoundMgr()
        {
            m_WaitToPlayBgms = new();
            m_PlayingSes = new();
            m_UnUsedSes = new();
        }

        public bool isBgmComplete
        {
            get
            {
                return m_BgmAudioSource != null && !m_BgmAudioSource.isPlaying && m_BgmAudioSource.clip != null;
            }
        }

        public event WuWuFrameworkAction onBgmFadeCompleteEvent
        {
            add
            {
                m_OnBgmFadeCompleteEvent += value;
            }
            remove
            {
                m_OnBgmFadeCompleteEvent -= value;
            }
        }


        public override void Update(float deltaTime, float unscaledDeltaTime, float time, float unscaledTime)
        {
            CheckBgm();
            CheckSe();
        }

        public override void Shutdown()
        {
            StopBgm(true);

            for (int i = m_PlayingSes.Count - 1; i > -1; i--)
            {
                PutSe(m_PlayingSes[i]);
            }

            ReleaseSeAudioSources();
            m_PlayingSes.Clear();
            m_IsBgmPause = false;
            m_OnBgmFadeCompleteEvent = null;
        }

        public void SetResourcePoolMgr(IResourcePoolMgr resourcePoolMgr)
        {
            m_ResourcePoolMgr = resourcePoolMgr;
            m_Root = new GameObject("SoundMgr");
            m_BgmAudioSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(WuWuFrameworkEntry.gameEntryObj.transform, false);
        }

        public void PlaySe(string assetPath, float volume = 1)
        {
            foreach (var playingSe in m_PlayingSes)
            {
                float process = Time.time - playingSe.playTime;

                if (assetPath.Equals(playingSe.assetPath) && process <= 0.05f)
                {
                    return;
                }
            }

            SeInfo seInfo = m_UnUsedSes.Count > 0 ? m_UnUsedSes.Dequeue() : new SeInfo(m_Root.transform);
            seInfo.SetSeInfo(assetPath, Time.time, volume);
            m_PlayingSes.Add(seInfo);
            InnerPlaySe(assetPath);
        }

        public void StopAllSes()
        {
            for (int i = m_PlayingSes.Count - 1; i >= 0; i--)
            {
                PutSe(m_PlayingSes[i]);
            }
        }

        public void SetSePlaySpeed(float speed)
        {
            if (m_PlayingSes is { Count: > 0 })
            {
                foreach (var playingSe in m_PlayingSes)
                {
                    if (playingSe.audioSource != null && playingSe.audioSource.isPlaying)
                    {
                        playingSe.audioSource.pitch = speed;
                    }
                }
            }
        }

        public void PlayBgmGroup(BgmInfo[] bgmGroup, bool isForcePlay = false)
        {
            if (!isForcePlay)
            {
                bool isAllInPlaying = true;

                foreach (var bgmInfo in bgmGroup)
                {
                    if (!IsBgmPlaying(bgmInfo.assetPath))
                    {
                        isAllInPlaying = false;
                        break;
                    }
                }

                if (isAllInPlaying)
                {
                    return;
                }
            }

            StopBgm(isForcePlay);

            foreach (var bgmInfo in bgmGroup)
            {
                m_WaitToPlayBgms.Enqueue(bgmInfo);
            }

            if (bgmGroup.Length > 1)
            {
                for (int i = 1; i < bgmGroup.Length; i++)
                {
                    m_ResourcePoolMgr.Cache<AudioClip>(bgmGroup[i].assetPath);
                }
            }
        }

        public void PlayBgm(string assetPath, bool isLoop, float volume = 1, float lerpTime = 0, bool isForcePlay = false)
        {
            if (!isForcePlay && IsBgmPlaying(assetPath))
            {
                return;
            }

            StopBgm(isForcePlay);
            m_WaitToPlayBgms.Enqueue(BgmInfo.Create(assetPath, isLoop, volume, lerpTime));
            m_ResourcePoolMgr.Cache<AudioClip>(assetPath);
        }

        public void StopBgm(bool isForceStop = false)
        {
            if (m_PlayingBgm != null)
            {
                m_BgmAudioSource.Stop();

                if (m_BgmAudioSource.clip != null)
                {
                    m_ResourcePoolMgr.Put(m_PlayingBgm.assetPath, m_BgmAudioSource.clip);
                }

                m_PlayingBgm.Release();
                m_PlayingBgm = null;
                m_BgmAudioSource.clip = null;
            }

            if (isForceStop)
            {
                m_WaitToPlayBgms.Clear();
            }
        }

        public void PauseBgm()
        {
            if (m_IsBgmPause)
            {
                return;
            }

            m_IsBgmPause = true;

            if (m_BgmAudioSource != null)
            {
                m_BgmAudioSource.Stop();
            }
        }

        public void ResumeBgm()
        {
            if (!m_IsBgmPause)
            {
                return;
            }

            m_IsBgmPause = false;

            if (m_BgmAudioSource != null)
            {
                m_BgmAudioSource.volume = 1f;
                m_BgmAudioSource.Play();
            }
        }

        public void FadeBgm(float endValue, float delay, float duration)
        {
            if (m_BgmAudioSource != null)
            {
                m_BgmAudioSource.DOFade(endValue, duration).SetEase(Ease.Linear).SetDelay(delay).OnComplete(OnBgmFadeComplete);
            }
        }

        public bool IsBgmPlaying(string assetPath)
        {
            if (m_IsBgmPause)
            {
                return false;
            }

            if (m_PlayingBgm != null && m_PlayingBgm.assetPath.Equals(assetPath))
            {
                return true;
            }

            foreach (BgmInfo audioGroup in m_WaitToPlayBgms)
            {
                if (audioGroup.assetPath.Equals(assetPath))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetBgmSpeed(float speed)
        {
            if (m_BgmAudioSource is not null)
            {
                m_BgmAudioSource.pitch = speed;
            }
        }

        public void ReleaseSeAudioSources()
        {
            while (m_UnUsedSes.Count > 0)
            {
                SeInfo seInfo = m_UnUsedSes.Dequeue();
                UnityObject.Destroy(seInfo.audioSource.gameObject);
            }

            m_UnUsedSes.Clear();
        }

        private void OnBgmFadeComplete()
        {
            m_OnBgmFadeCompleteEvent?.Invoke();
            m_OnBgmFadeCompleteEvent = null;
        }

        private void InnerPlaySe(string assetPath)
        {
            m_ResourcePoolMgr.Get<AudioClip>(assetPath, OnSeLoaded);
        }

        private void OnSeLoaded(string assetPath, UnityObject obj, object arg)
        {
            SeInfo playingSe = null;

            foreach (SeInfo seInfo in m_PlayingSes)
            {
                if (seInfo.assetPath == assetPath && seInfo.audioSource.clip == null)
                {
                    playingSe = seInfo;
                    break;
                }
            }

            if (playingSe == null)
            {
                m_ResourcePoolMgr.Put(assetPath, obj);
                return;
            }

            playingSe.audioSource.clip = obj as AudioClip;
            playingSe.audioSource.SetActiveSelf(true);
            playingSe.audioSource.Play();
        }

        private void InnerPlayBgm()
        {
            if (m_PlayingBgm == null)
            {
                return;
            }

            m_ResourcePoolMgr.Get<AudioClip>(m_PlayingBgm.assetPath, OnBgmLoaded);
        }

        private void OnBgmLoaded(string assetPath, UnityObject obj, object arg)
        {
            if (m_PlayingBgm == null)
            {
                m_ResourcePoolMgr.Put(assetPath, obj);
                return;
            }

            m_BgmAudioSource.clip = obj as AudioClip;
            m_BgmAudioSource.loop = m_PlayingBgm.isLoop;
            m_BgmAudioSource.volume = m_PlayingBgm.lerpTime > 0f ? 0f : m_PlayingBgm.volume;

            if (!m_IsBgmPause)
            {
                m_BgmAudioSource.Play();

                if (m_PlayingBgm.lerpTime > 0f)
                {
                    m_BgmAudioSource.DOFade(m_PlayingBgm.volume, m_PlayingBgm.lerpTime);
                }
            }
            else
            {
                m_BgmAudioSource.Pause();
            }
        }

        private void PutSe(SeInfo seInfo)
        {
            m_ResourcePoolMgr.Put(seInfo.assetPath, seInfo.audioSource.clip);
            m_PlayingSes.Remove(seInfo);
            m_UnUsedSes.Enqueue(seInfo);
            seInfo.Clear();
        }

        private void CheckBgm()
        {
            if (m_IsBgmPause)
            {
                return;
            }

            if (m_PlayingBgm == null && m_WaitToPlayBgms.Count > 0)
            {
                m_PlayingBgm = m_WaitToPlayBgms.Dequeue();
                InnerPlayBgm();
            }

            if (m_PlayingBgm != null && m_BgmAudioSource.clip != null && !m_PlayingBgm.isLoop)
            {
                if (!m_BgmAudioSource.isPlaying)
                {
                    StopBgm();
                }
            }
        }

        private void CheckSe()
        {
            if (m_PlayingSes.Count < 1)
            {
                return;
            }

            for (int i = m_PlayingSes.Count - 1; i >= 0; i--)
            {
                if (!m_PlayingSes[i].audioSource.isPlaying && m_PlayingSes[i].audioSource.clip is not null)
                {
                    PutSe(m_PlayingSes[i]);
                }
            }
        }
    }
}