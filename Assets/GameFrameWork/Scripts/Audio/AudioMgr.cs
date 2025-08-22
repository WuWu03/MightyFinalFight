using DG.Tweening;
using GameFrameWork.Pool;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork.Audio
{
    public class AudioMgr : BaseMgr<AudioMgr>
    {
        public bool isBgmComplete
        {
            get
            {
                return m_BgmAudioSource != null && !m_BgmAudioSource.isPlaying && m_BgmAudioSource.clip != null;
            }
        }

        public event GameFrameWorkAction onBgmFadeCompleteEvent
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

        protected override void OnAwake()
        {
            m_Root = new GameObject("AudioMgr");
            m_BgmAudioSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            m_WaitToPlayBGMs = new();
            m_PlayingSes = new();
            m_UnUsedSes = new();
        }

        protected override void OnFixedUpdate()
        {
            CheckBgm();
            CheckSe();
        }

        protected override void OnShutDown()
        {
            StopBgm(true);

            for (int i = m_PlayingSes.Count - 1; i > -1; i--)
            {
                PutSe(m_PlayingSes[i]);
            }

            ReleaseSeAudioSources();
            m_PlayingSes.Clear();
        }

        protected override void OnDestory()
        {
            m_IsBgmPause = false;
            m_OnBgmFadeCompleteEvent = null;
            m_BgmAudioSource = null;
            m_PlayingBgm = null;
            m_WaitToPlayBGMs = null;
            m_PlayingSes = null;
            m_UnUsedSes = null;
        }

        public void PlaySe(string sePath, float volume = 1)
        {
            for (int i = 0; i < m_PlayingSes.Count; i++)
            {
                string path = m_PlayingSes[i].path;
                float process = Time.time - m_PlayingSes[i].playTime;
                if (sePath.Equals(path) && process <= 0.05f)
                {
                    return;
                }
            }

            GetSe(sePath, volume);
            InnerPlaySe(sePath, volume);
        }

        public void StopAllSe()
        {
            for (int i = m_PlayingSes.Count - 1; i >= 0; i--)
            {
                PutSe(m_PlayingSes[i]);
            }
        }

        public void SetSePlaySpeed(float speed)
        {
            if(m_PlayingSes != null && m_PlayingSes.Count > 0)
            {
                for (int i = 0; i < m_PlayingSes.Count; i++)
                {
                    if(m_PlayingSes[i].audioSource != null && m_PlayingSes[i].audioSource.isPlaying)
                    {
                        m_PlayingSes[i].audioSource.pitch = speed;
                    }
                }
            }
        }

        public void PlayBgmGroup(BgmInfo[] bgmGroup, bool isForcePlay = false)
        {
            if (!isForcePlay)
            {
                bool isAllInPlaying = true;

                for (int i = 0; i < bgmGroup.Length; i++)
                {
                    if (!IsBGMPlaying(bgmGroup[i].assetPath))
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

            for (int i = 0; i < bgmGroup.Length; i++)
            {
                m_WaitToPlayBGMs.Enqueue(bgmGroup[i]);
            }

            if (bgmGroup.Length > 1)
            {
                for (int i = 1; i < bgmGroup.Length; i++)
                {
                    AssetsPool.instance.Cache<AudioClip>(bgmGroup[i].assetPath);
                }
            }
        }

        public void PlayBgm(string assetPath, bool isLoop, float volum = 1, float lerpTime = 0, bool isForcePlay = false)
        {
            if (!isForcePlay && IsBGMPlaying(assetPath))
            {
                return;
            }

            StopBgm(isForcePlay);
            m_WaitToPlayBGMs.Enqueue(BgmInfo.Create(assetPath, isLoop, volum, lerpTime));
            AssetsPool.instance.Cache<AudioClip>(assetPath);
        }

        public void StopBgm(bool isForceStop = false)
        {
            if (m_PlayingBgm != null)
            {
                m_BgmAudioSource.Stop();
                AssetsPool.instance.Put(m_PlayingBgm.assetPath, m_BgmAudioSource.clip);
                m_PlayingBgm.Release();
                m_PlayingBgm = null;
                m_BgmAudioSource.clip = null;
            }

            if (isForceStop)
            {
                m_WaitToPlayBGMs.Clear();
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
                m_BgmAudioSource.DOFade(endValue, duration).SetEase(Ease.Linear).SetDelay(delay).OnComplete(OnBGMFadeComplete);
            }
        }

        public bool IsBGMPlaying(string assetPath)
        {
            if(m_IsBgmPause)
            {
                return false;
            }

            if (m_PlayingBgm != null && m_PlayingBgm.assetPath.Equals(assetPath))
            {
                return true;
            }

            foreach(BgmInfo audioGroup in m_WaitToPlayBGMs)
            {
                if (audioGroup.assetPath.Equals(assetPath))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetBGMSpeed(float speed)
        {
            if(m_BgmAudioSource != null)
            {
                m_BgmAudioSource.pitch = speed;
            }
        }

        public void ReleaseSeAudioSources()
        {
            while (m_UnUsedSes.Count > 0)
            {
                SeInfo seInfo = m_UnUsedSes.Dequeue();
                GameObject.Destroy(seInfo.audioSource.gameObject);
            }

            m_UnUsedSes.Clear();
        }

        private void OnBGMFadeComplete()
        {
            m_OnBgmFadeCompleteEvent?.Invoke();
            m_OnBgmFadeCompleteEvent = null;
        }

        private void InnerPlaySe(string assetPath,float volume)
        {
            AssetsPool.instance.Get<AudioClip>(assetPath, OnSeLoaded, AudioSourceInfo.Create(volume, 0, false));
        }

        private void OnSeLoaded(string assetPath, UnityEngine.Object obj, object arg)
        {
            AudioSourceInfo audioSourceInfo = arg as AudioSourceInfo;
            SeInfo seInfo = null;

            for (int i = 0; i < m_PlayingSes.Count; i++)
            {
                SeInfo tempSoundEffectInfo = m_PlayingSes[i];
                if (tempSoundEffectInfo.path == assetPath && tempSoundEffectInfo.audioSource.clip == null)
                {
                    seInfo = m_PlayingSes[i];
                }
            }

            seInfo ??= GetSe(assetPath, audioSourceInfo.volume);
            seInfo.audioSource.clip = obj as AudioClip;
            seInfo.audioSource.SetActiveSelf(true);
            seInfo.audioSource.Play();
            audioSourceInfo.Release();
        }

        private void InnerPlayBgm(string assetPath, float volum, float fadeTime, bool isLoop)
        {
            AssetsPool.instance.Get<AudioClip>(assetPath, OnBgmLoaded, AudioSourceInfo.Create(volum, fadeTime, isLoop));
        }

        private void OnBgmLoaded(string assetPath, UnityEngine.Object obj, object arg)
        {
            AudioSourceInfo audioSourceInfo = arg as AudioSourceInfo;

            m_BgmAudioSource.clip = obj as AudioClip;
            m_BgmAudioSource.loop = audioSourceInfo.isLoop;
            m_BgmAudioSource.volume = audioSourceInfo.fadeTime > 0f ? 0f : audioSourceInfo.volume;

            if (!m_IsBgmPause)
            {
                m_BgmAudioSource.Play();

                if (audioSourceInfo.fadeTime > 0f)
                {
                    m_BgmAudioSource.DOFade(audioSourceInfo.volume, audioSourceInfo.fadeTime);
                }
            }
            else
            {
                m_BgmAudioSource.Pause();
            }

            audioSourceInfo.Release();
        }

        private SeInfo GetSe(string assetPath, float volume)
        {
            SeInfo seInfo = null;

            if (m_UnUsedSes.Count > 0)
            {
                seInfo = m_UnUsedSes.Dequeue();
            }

            if (seInfo == null)
            {
                seInfo = SeInfo.Create();
                seInfo.audioSource.transform.SetParent(m_Root.transform, false);
            }

            m_PlayingSes.Add(seInfo);

            seInfo.path = assetPath;
            seInfo.playTime = Time.time;
            seInfo.audioSource.SetActiveSelf(false);
            seInfo.audioSource.name = Path.GetFileNameWithoutExtension(assetPath);
            seInfo.audioSource.volume = volume;
            seInfo.audioSource.playOnAwake = false;
            seInfo.audioSource.loop = false;
            seInfo.audioSource.Stop();

            return seInfo;
        }

        private void PutSe(SeInfo seInfo)
        {
            AssetsPool.instance.Put(seInfo.path, seInfo.audioSource.clip);
            seInfo.Clear();
            seInfo.audioSource.clip = null;
            seInfo.audioSource.Stop();
            seInfo.audioSource.SetActiveSelf(false);
            m_PlayingSes.Remove(seInfo);
            m_UnUsedSes.Enqueue(seInfo);
        }

        private void CheckBgm()
        {
            if(m_IsBgmPause)
            {
                return;
            }

            if (m_PlayingBgm == null && m_WaitToPlayBGMs.Count > 0)
            {
                m_PlayingBgm = m_WaitToPlayBGMs.Dequeue();
                InnerPlayBgm(m_PlayingBgm.assetPath, m_PlayingBgm.volume, m_PlayingBgm.lerpTime, m_PlayingBgm.isLoop);
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
                if (!m_PlayingSes[i].audioSource.isPlaying && m_PlayingSes[i].audioSource.clip != null)
                {
                    PutSe(m_PlayingSes[i]);
                }
            }
        }

        private bool m_IsBgmPause = false;
        private event GameFrameWorkAction m_OnBgmFadeCompleteEvent = null;
        private AudioSource m_BgmAudioSource = null;
        private BgmInfo m_PlayingBgm = null;
        private Queue<BgmInfo> m_WaitToPlayBGMs = null;
        private List<SeInfo> m_PlayingSes = null;
        private Queue<SeInfo> m_UnUsedSes = null;
        private GameObject m_Root = null;
    }
}