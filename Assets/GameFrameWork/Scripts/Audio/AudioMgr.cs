using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameFrameWork.Utilities;

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
            m_Root = new GameObject("SoundMgr");
            m_BGMAudioSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            m_QueueBGMAudioGroup = new Queue<AudioGroup>();
            m_ListPlayingSE = new List<AudioSEInfo>();
            m_StackSE = new Stack<AudioSEInfo>();
        }

        public void PlaySE(string path, string name, float volume = 1)
        {
            for (int i = 0; i < m_ListPlayingSE.Count; i++)
            {
                string sePath = m_ListPlayingSE[i].path;
                string seName = m_ListPlayingSE[i].name;
                float process = Time.time - m_ListPlayingSE[i].playTime;
                if (sePath.Equals(path) && seName.Equals(name) && process <= 0.05f)
                {
                    return;
                }
            }

            InnerPlaySE(path, name, volume);
        }

        public void SetSEPlaySpeed(float speed)
        {
            if(m_ListPlayingSE != null && m_ListPlayingSE.Count > 0)
            {
                for (int i = 0; i < m_ListPlayingSE.Count; i++)
                {
                    if(m_ListPlayingSE[i].audioSource != null && m_ListPlayingSE[i].audioSource.isPlaying)
                    {
                        m_ListPlayingSE[i].audioSource.pitch = speed;
                    }
                }
            }
        }

        public void PlayBGMGroup(AudioGroup[] audioGroups, bool isForcePlay = false)
        {
            if (!isForcePlay)
            {
                bool isAllInPlaying = true;

                for (int i = 0; i < audioGroups.Length; i++)
                {
                    if (!IsBGMPlaying(audioGroups[i].GetPath()))
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
                m_QueueBGMAudioGroup.Enqueue(audioGroups[i]);
            }

            if (audioGroups.Length > 1)
            {
                for (int i = 1; i < audioGroups.Length; i++)
                {
                    //ResourcesMgr.instance.LoadAssetAsync
                    ResourcesPool.instance.Cache<AudioClip>(audioGroups[i].GetPath());
                }
            }
        }

        public void PlayBGM(string path, string name, bool isLoop, float volum = 1, float lerpTime = 0, bool isForcePlay = false)
        {
            if (!isForcePlay && IsBGMPlaying(PathUtil.FormatPath(path, name)))
            {
                return;
            }

            StopBGM(isForcePlay);

            m_QueueBGMAudioGroup.Clear();
            m_QueueBGMAudioGroup.Enqueue(AudioGroup.Create(path, name, isLoop, volum, lerpTime));
        }

        public void StopBGM()
        {
            StopBGM(true);
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

        public bool IsBGMPlaying(string fullName)
        {
            if(m_IsBGMPause)
            {
                return false;
            }

            if (m_BGMAudioGroup != null && m_BGMAudioGroup.GetPath().Equals(fullName))
            {
                return true;
            }

            foreach(AudioGroup group in m_QueueBGMAudioGroup)
            {
                if(group.GetPath().Equals(fullName)) return true;
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

        private void OnBGMFadeComplete()
        {
            m_OnBGMFadeCompleteEvent?.Invoke();
            m_OnBGMFadeCompleteEvent = null;
        }

        private void InnerPlaySE(string path, string name, float volume)
        {
            ResourcesPool.instance.Get<AudioClip>(PathUtil.FormatPath(path, name), OnSELoaded, path, name, volume);
        }

        private void OnSELoaded(string assetPath, UnityEngine.Object obj, object[] param)
        {
            string path = (string)param[0];
            string name = (string)param[1];
            float volume = (float)param[2];

            AudioSEInfo audioSEInfo = GetSE(path, name, volume);
            audioSEInfo.audioSource.clip = obj as AudioClip;
            audioSEInfo.audioSource.Play();

            m_ListPlayingSE.Add(audioSEInfo);
        }

        private void InnerPlayBGM(string path, float volum, float fadeTime, bool isLoop)
        {
            ResourcesPool.instance.Get<AudioClip>(path, OnBGMLoaded, volum, fadeTime, isLoop);
        }

        private void OnBGMLoaded(string assetPath, UnityEngine.Object obj, object[] param)
        {
            float volume = (float)param[0];
            float fadeTime = (float)param[1];
            bool isLoop = (bool)param[2];

            m_BGMAudioSource.clip = obj as AudioClip;
            m_BGMAudioSource.loop = isLoop;
            m_BGMAudioSource.volume = fadeTime > 0f ? 0f : volume;

            if (!m_IsBGMPause)
            {
                m_BGMAudioSource.Play();

                if (fadeTime > 0f)
                {
                    m_BGMAudioSource.DOFade(volume, fadeTime);
                }
            }
            else
            {
                m_BGMAudioSource.Pause();
            }
        }

        private AudioSEInfo GetSE(string path, string name, float volume)
        {
            AudioSEInfo audioSEInfo = null;

            if (m_StackSE.Count > 0)
            {
                audioSEInfo = m_StackSE.Pop();
            }
            else
            {
                audioSEInfo = AudioSEInfo.Create();
                audioSEInfo.audioSource.transform.SetParent(m_Root.transform, false);
            }

            audioSEInfo.path = path;
            audioSEInfo.name = name;
            audioSEInfo.playTime = Time.time;
            audioSEInfo.audioSource.SetActive(true);
            audioSEInfo.audioSource.name = name;
            audioSEInfo.audioSource.volume = volume;
            audioSEInfo.audioSource.playOnAwake = false;
            audioSEInfo.audioSource.loop = false;
            audioSEInfo.audioSource.Stop();

            return audioSEInfo;
        }

        private void PutSE(AudioSEInfo audioSEInfo)
        {
            ResourcesPool.instance.Put(audioSEInfo.GetResPath(), audioSEInfo.audioSource.clip);
            audioSEInfo.Clear();
            audioSEInfo.audioSource.clip = null;
            audioSEInfo.audioSource.Stop();
            audioSEInfo.audioSource.SetActive(false);
            m_StackSE.Push(audioSEInfo);
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

            if (m_BGMAudioGroup == null && m_QueueBGMAudioGroup.Count > 0)
            {
                m_BGMAudioGroup = m_QueueBGMAudioGroup.Dequeue();
                InnerPlayBGM(m_BGMAudioGroup.GetPath(), m_BGMAudioGroup.volume, m_BGMAudioGroup.lerpTime, m_BGMAudioGroup.isLoop);
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
            if (m_ListPlayingSE.Count < 1)
            {
                return;
            }

            for (int i = m_ListPlayingSE.Count - 1; i >= 0; i--)
            {
                if (!m_ListPlayingSE[i].audioSource.isPlaying && m_ListPlayingSE[i].audioSource.clip != null)
                {
                    PutSE(m_ListPlayingSE[i]);
                    m_ListPlayingSE.RemoveAt(i);
                }
            }
        }

        private void StopBGM(bool isForceStop = false)
        {
            if (m_BGMAudioGroup != null)
            {
                ResourcesPool.instance.Put(m_BGMAudioGroup.GetPath(), m_BGMAudioSource.clip);
                ReferencePool.Release(m_BGMAudioGroup);
                m_BGMAudioSource.Stop();
                m_BGMAudioGroup = null;
                m_BGMAudioSource.clip = null;
            }

            if(isForceStop)
            {
                m_QueueBGMAudioGroup.Clear();
            }
        }

        protected override void OnShutDown()
        {
            StopBGM(true);

            for (int i = 0; i < m_ListPlayingSE.Count; i++)
            {
                PutSE(m_ListPlayingSE[i]);
            }

            while(m_StackSE.Count > 0)
            {
                AudioSEInfo audioSEInfo = m_StackSE.Pop();
                ResourcesPool.instance.Put(audioSEInfo.GetResPath(), audioSEInfo.audioSource.clip);
                GameObject.Destroy(audioSEInfo.audioSource.gameObject);
            }

            m_ListPlayingSE.Clear();
            m_StackSE.Clear();
        }

        private bool m_IsBGMPause = false;
        private event GameFrameWorkAction m_OnBGMFadeCompleteEvent = null;
        private AudioSource m_BGMAudioSource = null;
        private AudioGroup m_BGMAudioGroup = null;
        private Queue<AudioGroup> m_QueueBGMAudioGroup = null;
        private List<AudioSEInfo> m_ListPlayingSE = null;
        private Stack<AudioSEInfo> m_StackSE = null;
        private GameObject m_Root = null;
    }
}