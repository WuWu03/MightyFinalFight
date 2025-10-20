using System;
using DG.Tweening;
using GameFrameWork.Pool;
using System.Collections.Generic;
using System.IO;
using GameFrameWork.Event;
using GameFrameWork.Utils;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace GameFrameWork.Audio
{
    public class SoundMgr : GameFrameWorkModule,ISoundMgr
    {
        private readonly Queue<BgmInfo> m_WaitToPlayBgms;
        private readonly List<SeInfo> m_PlayingSes;
        private readonly Queue<SeInfo> m_UnUsedSes;
        private bool m_IsBgmPause;
        private AudioSource m_BgmAudioSource;
        private BgmInfo m_PlayingBgm;
        private GameObject m_Root;
        private IResourcePoolMgr m_ResourcePoolMgr;
        private event GameFrameWorkAction m_OnBgmFadeCompleteEvent;
        
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

        public void SetResourcePoolMgr(IResourcePoolMgr resourcePoolMgr, Transform root)
        {
            m_ResourcePoolMgr = resourcePoolMgr;
            m_Root = new GameObject("SoundMgr");
            m_BgmAudioSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(root, false);
        }

        public void PlaySe(string sePath, float volume = 1)
        {
            foreach (var playingSE in m_PlayingSes)
            {
                string path = playingSE.path;
                float process = Time.time - playingSE.playTime;
                if (sePath.Equals(path) && process <= 0.05f)
                {
                    return;
                }
            }

            GetSe(sePath, volume);
            InnerPlaySe(sePath, volume);
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
            if(m_PlayingSes is { Count: > 0 })
            {
                foreach (var playingSe in m_PlayingSes)
                {
                    if(playingSe.audioSource != null && playingSe.audioSource.isPlaying)
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
                m_ResourcePoolMgr.Put(m_PlayingBgm.assetPath, m_BgmAudioSource.clip);
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
            if(m_IsBgmPause)
            {
                return false;
            }

            if (m_PlayingBgm != null && m_PlayingBgm.assetPath.Equals(assetPath))
            {
                return true;
            }

            foreach(BgmInfo audioGroup in m_WaitToPlayBgms)
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
            if(m_BgmAudioSource is not null)
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

        private void InnerPlaySe(string assetPath,float volume)
        {
            m_ResourcePoolMgr.Get<AudioClip>(assetPath, OnSeLoaded, AudioSourceInfo.Create(volume, 0, false));
        }

        private void OnSeLoaded(string assetPath, UnityObject obj, object arg)
        {
            AudioSourceInfo audioSourceInfo = arg as AudioSourceInfo;

            if (audioSourceInfo == null)
            {
                throw new Exception(StringUtil.Append("[", assetPath, "] ", "音效数据丢失"));
            }
            
            SeInfo seInfo = null;

            foreach (var playingSe in m_PlayingSes)
            {
                SeInfo tempSoundEffectInfo = playingSe;
                if (tempSoundEffectInfo.path == assetPath && tempSoundEffectInfo.audioSource.clip == null)
                {
                    seInfo = playingSe;
                }
            }

            seInfo ??= GetSe(assetPath, audioSourceInfo.volume);
            seInfo.audioSource.clip = obj as AudioClip;
            seInfo.audioSource.SetActiveSelf(true);
            seInfo.audioSource.Play();
            audioSourceInfo.Release();
        }

        private void InnerPlayBgm(string assetPath, float volume, float fadeTime, bool isLoop)
        {
            m_ResourcePoolMgr.Get<AudioClip>(assetPath, OnBgmLoaded, AudioSourceInfo.Create(volume, fadeTime, isLoop));
        }

        private void OnBgmLoaded(string assetPath, UnityObject obj, object arg)
        {
            AudioSourceInfo audioSourceInfo = arg as AudioSourceInfo;

            if (audioSourceInfo == null)
            {
                throw new Exception(StringUtil.Append("[", assetPath, "] ", "背景音乐数据丢失"));
            }
            
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
            m_ResourcePoolMgr.Put(seInfo.path, seInfo.audioSource.clip);
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

            if (m_PlayingBgm == null && m_WaitToPlayBgms.Count > 0)
            {
                m_PlayingBgm = m_WaitToPlayBgms.Dequeue();
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
                if (!m_PlayingSes[i].audioSource.isPlaying && m_PlayingSes[i].audioSource.clip is not null)
                {
                    PutSe(m_PlayingSes[i]);
                }
            }
        }
    }
}