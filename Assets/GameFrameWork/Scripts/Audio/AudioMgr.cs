using DG.Tweening;
using GameFrameWork.Pool;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameWork.Audio
{
    public class AudioMgr : BaseMgr<AudioMgr>
    {
        public event GameFrameWorkAction onBGMFadeCompleteEvent
        {
            add
            {
                m_OnBGMFadeCompleteEvent += value;
            }
            remove
            {
                m_OnBGMFadeCompleteEvent -= value;
            }
        }

        protected override void OnAwake()
        {
            m_Root = new GameObject("AudioMgr");
            m_BGMAudioSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            m_BGMAudioGroupQueue = new Queue<BGMInfo>();
            m_PlayingSEList = new List<SEInfo>();
            m_SoundEffectQueue = new Queue<SEInfo>();
        }

        public void PlaySE(string sePath, float volume = 1)
        {
            for (int i = 0; i < m_PlayingSEList.Count; i++)
            {
                string path = m_PlayingSEList[i].path;
                float process = Time.time - m_PlayingSEList[i].playTime;
                if (sePath.Equals(path) && process <= 0.05f)
                {
                    return;
                }
            }

            GetSE(sePath, volume);
            InnerPlaySE(sePath, volume);
        }

        public void SetSEPlaySpeed(float speed)
        {
            if(m_PlayingSEList != null && m_PlayingSEList.Count > 0)
            {
                for (int i = 0; i < m_PlayingSEList.Count; i++)
                {
                    if(m_PlayingSEList[i].audioSource != null && m_PlayingSEList[i].audioSource.isPlaying)
                    {
                        m_PlayingSEList[i].audioSource.pitch = speed;
                    }
                }
            }
        }

        public void PlayBGMGroup(BGMInfo[] audioGroups, bool isForcePlay = false)
        {
            if (!isForcePlay)
            {
                bool isAllInPlaying = true;

                for (int i = 0; i < audioGroups.Length; i++)
                {
                    if (!IsBGMPlaying(audioGroups[i].assetPath))
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

            StopBGM(isForcePlay);

            for (int i = 0; i < audioGroups.Length; i++)
            {
                m_BGMAudioGroupQueue.Enqueue(audioGroups[i]);
            }

            if (audioGroups.Length > 1)
            {
                for (int i = 1; i < audioGroups.Length; i++)
                {
                    AssetsPool.instance.Cache<AudioClip>(audioGroups[i].assetPath);
                }
            }
        }

        public void PlayBGM(string assetPath, bool isLoop, float volum = 1, float lerpTime = 0, bool isForcePlay = false)
        {
            if (!isForcePlay && IsBGMPlaying(assetPath))
            {
                return;
            }

            StopBGM(isForcePlay);
            m_BGMAudioGroupQueue.Enqueue(BGMInfo.Create(assetPath, isLoop, volum, lerpTime));
        }

        public void StopBGM(bool isForceStop = false)
        {
            if (m_BGMAudioGroup != null)
            {
                AssetsPool.instance.Put(m_BGMAudioGroup.assetPath, m_BGMAudioSource.clip);
                ReferencePool.ReleaseReference(m_BGMAudioGroup);
                m_BGMAudioSource.Stop();
                m_BGMAudioGroup = null;
                m_BGMAudioSource.clip = null;
            }

            if (isForceStop)
            {
                m_BGMAudioGroupQueue.Clear();
            }
        }

        public void StartBGM()
        {
            if (!m_IsBGMPause)
            {
                return;
            }

            m_IsBGMPause = false;

            if (m_BGMAudioSource != null)
            {
                m_BGMAudioSource.Play();
            }
        }

        public void PauseBGM()
        {
            if (m_IsBGMPause)
            {
                return;
            }

            m_IsBGMPause = true;

            if (m_BGMAudioSource != null)
            {
                m_BGMAudioSource.Stop();
            }
        }

        public void FadeBGM(float endValue, float delay, float duration)
        {
            if (m_BGMAudioSource != null)
            {
                m_BGMAudioSource.DOFade(endValue, duration).SetEase(Ease.Linear).SetDelay(delay).OnComplete(OnBGMFadeComplete);
            }
        }

        public bool IsBGMPlaying(string assetPath)
        {
            if(m_IsBGMPause)
            {
                return false;
            }

            if (m_BGMAudioGroup != null && m_BGMAudioGroup.assetPath.Equals(assetPath))
            {
                return true;
            }

            foreach(BGMInfo audioGroup in m_BGMAudioGroupQueue)
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
            if(m_BGMAudioSource != null)
            {
                m_BGMAudioSource.pitch = speed;
            }
        }

        public void ReleaseAuioClips()
        {
            while (m_SoundEffectQueue.Count > 0)
            {
                SEInfo seInfo = m_SoundEffectQueue.Dequeue();
                GameObject.Destroy(seInfo.audioSource.gameObject);
            }

            m_SoundEffectQueue.Clear();
        }

        private void OnBGMFadeComplete()
        {
            m_OnBGMFadeCompleteEvent?.Invoke();
            m_OnBGMFadeCompleteEvent = null;
        }

        private void InnerPlaySE(string assetPath,float volume)
        {
            AssetsPool.instance.Get<AudioClip>(assetPath, OnSELoaded, AudioSourceInfo.Create(volume, 0, false));
        }

        private void OnSELoaded(string assetPath, UnityEngine.Object obj, object[] param)
        {
            AudioSourceInfo audioSourceInfo = (AudioSourceInfo)param[0];
            SEInfo seInfo = null;

            for (int i = 0; i < m_PlayingSEList.Count; i++)
            {
                SEInfo tempSoundEffectInfo = m_PlayingSEList[i];
                if (tempSoundEffectInfo.path == assetPath && tempSoundEffectInfo.audioSource.clip == null)
                {
                    seInfo = m_PlayingSEList[i];
                }
            }

            if (seInfo == null)
            {
                seInfo = GetSE(assetPath, audioSourceInfo.volume);
            }

            seInfo.audioSource.clip = obj as AudioClip;
            seInfo.audioSource.SetActiveSelf(true);
            seInfo.audioSource.Play();
            ReferencePool.ReleaseReference(audioSourceInfo);
        }

        private void InnerPlayBGM(string assetPath, float volum, float fadeTime, bool isLoop)
        {
            AssetsPool.instance.Get<AudioClip>(assetPath, OnBGMLoaded, AudioSourceInfo.Create(volum, fadeTime, isLoop));
        }

        private void OnBGMLoaded(string assetPath, UnityEngine.Object obj, object[] param)
        {
            AudioSourceInfo audioSourceInfo = (AudioSourceInfo)param[0];

            m_BGMAudioSource.clip = obj as AudioClip;
            m_BGMAudioSource.loop = audioSourceInfo.isLoop;
            m_BGMAudioSource.volume = audioSourceInfo.fadeTime > 0f ? 0f : audioSourceInfo.volume;

            if (!m_IsBGMPause)
            {
                m_BGMAudioSource.Play();

                if (audioSourceInfo.fadeTime > 0f)
                {
                    m_BGMAudioSource.DOFade(audioSourceInfo.volume, audioSourceInfo.fadeTime);
                }
            }
            else
            {
                m_BGMAudioSource.Pause();
            }

            ReferencePool.ReleaseReference(audioSourceInfo);
        }

        private SEInfo GetSE(string assetPath, float volume)
        {
            SEInfo seInfo = null;

            if (m_SoundEffectQueue.Count > 0)
            {
                seInfo = m_SoundEffectQueue.Dequeue();
            }

            if (seInfo == null)
            {
                seInfo = SEInfo.Create();
                seInfo.audioSource.transform.SetParent(m_Root.transform, false);
            }

            m_PlayingSEList.Add(seInfo);

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

        private void PutSE(SEInfo seInfo)
        {
            AssetsPool.instance.Put(seInfo.path, seInfo.audioSource.clip);
            seInfo.Clear();
            seInfo.audioSource.clip = null;
            seInfo.audioSource.Stop();
            seInfo.audioSource.SetActiveSelf(false);
            m_PlayingSEList.Remove(seInfo);
            m_SoundEffectQueue.Enqueue(seInfo);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            CheckBGM();
            CheckSE();
        }

        private void CheckBGM()
        {
            if(m_IsBGMPause)
            {
                return;
            }

            if (m_BGMAudioGroup == null && m_BGMAudioGroupQueue.Count > 0)
            {
                m_BGMAudioGroup = m_BGMAudioGroupQueue.Dequeue();
                InnerPlayBGM(m_BGMAudioGroup.assetPath, m_BGMAudioGroup.volume, m_BGMAudioGroup.lerpTime, m_BGMAudioGroup.isLoop);
            }

            if (m_BGMAudioGroup != null && m_BGMAudioSource.clip != null && !m_BGMAudioGroup.isLoop)
            {
                if (!m_BGMAudioSource.isPlaying)
                {
                    StopBGM();
                }
            }
        }

        private void CheckSE()
        {
            if (m_PlayingSEList.Count < 1)
            {
                return;
            }

            for (int i = m_PlayingSEList.Count - 1; i >= 0; i--)
            {
                if (!m_PlayingSEList[i].audioSource.isPlaying && m_PlayingSEList[i].audioSource.clip != null)
                {
                    PutSE(m_PlayingSEList[i]);
                }
            }
        }

        protected override void OnShutDown()
        {
            base.OnShutDown();
            StopBGM(true);

            for (int i = m_PlayingSEList.Count - 1; i > -1; i--)
            {
                PutSE(m_PlayingSEList[i]);
            }

            ReleaseAuioClips();
            m_PlayingSEList.Clear();
            m_PlayingSEList = null;
            m_SoundEffectQueue = null;
        }

        private bool m_IsBGMPause = false;
        private event GameFrameWorkAction m_OnBGMFadeCompleteEvent = null;
        private AudioSource m_BGMAudioSource = null;
        private BGMInfo m_BGMAudioGroup = null;
        private Queue<BGMInfo> m_BGMAudioGroupQueue = null;
        private List<SEInfo> m_PlayingSEList = null;
        private Queue<SEInfo> m_SoundEffectQueue = null;
        private GameObject m_Root = null;
    }
}