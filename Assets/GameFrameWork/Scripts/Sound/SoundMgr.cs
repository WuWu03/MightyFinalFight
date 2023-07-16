using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameFrameWork.Utilities;

namespace GameFrameWork.Sound
{
    public class SoundMgr : BaseMgr<SoundMgr>
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
            m_BGMSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            m_QueueAudioGroup = new Queue<AudioGroup>();
            m_PlayingList = new List<AudioSoundPlay>();
            m_SoundStack = new Stack<AudioSoundPlay>();
        }

        public void PlaySound(string path, string name, float volume = 1)
        {
            for (int i = 0; i < m_PlayingList.Count; i++)
            {
                string soundPath = m_PlayingList[i].path;
                string soundName = m_PlayingList[i].name;
                float process = Time.time - m_PlayingList[i].playTime;
                if (soundPath.Equals(path) && soundName.Equals(name) && process <= 0.05f)
                {
                    return;
                }
            }

            InnerPlaySound(path, name, volume);
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

            StopCurrent(isForcePlay);

            for (int i = 0; i < audioGroups.Length; i++)
            {
                m_QueueAudioGroup.Enqueue(audioGroups[i]);
            }

            if (audioGroups.Length > 1)
            {
                for (int i = 1; i < audioGroups.Length; i++)
                {
                    AudioClipPool.instance.Get(audioGroups[i].GetPath(), null);
                }
            }
        }

        public void PlayBGM(string path, string name, bool isLoop, float volum = 1, float lerpTime = 0, bool isForcePlay = false)
        {
            if (!isForcePlay && IsBGMPlaying(PathUtil.FormatPath(path, name)))
            {
                return;
            }

            StopCurrent(isForcePlay);

            m_QueueAudioGroup.Clear();
            m_QueueAudioGroup.Enqueue(AudioGroup.Create(path, name, isLoop, volum, lerpTime));
        }

        public void StopBGM()
        {
            StopCurrent(true);
        }

        public void StartBGM()
        {
            if (!m_IsBGMPause)
            {
                return;
            }

            m_IsBGMPause = false;

            if (m_CurrPlayAudio == null)
            {
                m_PlayStamp = Time.time - m_StopStamp;
            }

            if (m_BGMSource != null)
            {
                m_BGMSource.Play();
            }
        }

        public void PauseBGM()
        {
            if (m_IsBGMPause)
            {
                return;
            }

            m_IsBGMPause = true;

            if (m_CurrPlayAudio == null)
            {
                m_StopStamp = Time.time - m_PlayStamp;
            }

            if (m_BGMSource != null)
            {
                m_BGMSource.Stop();
            }
        }

        public void FadeBGM(float endValue, float delay, float duration)
        {
            if (m_BGMSource != null)
            {
                m_BGMSource.DOFade(endValue, duration).SetEase(Ease.Linear).SetDelay(delay).OnComplete(OnBGMFadeComplete);
            }
        }

        public bool IsBGMPlaying(string fullName)
        {
            if(m_IsBGMPause)
            {
                return false;
            }

            if (m_CurrPlayAudio != null && m_CurrPlayAudio.GetPath().Equals(fullName))
            {
                return true;
            }

            foreach(AudioGroup group in m_QueueAudioGroup)
            {
                if(group.GetPath().Equals(fullName)) return true;
            }

            return false;
        }

        private void OnBGMFadeComplete()
        {
            m_OnBGMFadeCompleteEvent?.Invoke();
            m_OnBGMFadeCompleteEvent = null;
        }

        private void InnerPlaySound(string path, string name, float volume)
        {
            AudioClipPool.instance.Get(PathUtil.FormatPath(path, name), OnSoundLoaded, path, name, volume);
        }

        private void OnSoundLoaded(AudioClip clip, object[] param)
        {
            string path = (string)param[0];
            string name = (string)param[1];
            float volume = (float)param[2];

            AudioSoundPlay audioSoundPlay = GetSoundSource(path, name, volume);
            audioSoundPlay.audioSource.clip = clip;
            audioSoundPlay.audioSource.Play();

            m_PlayingList.Add(audioSoundPlay);
        }

        private void InnerPlayBGM(string path, float volum, float fadeTime, bool isLoop)
        {
            AudioClipPool.instance.Get(path, OnBGMLoaded, volum, fadeTime, isLoop);
        }

        private void OnBGMLoaded(AudioClip clip, object[] param)
        {
            float volume = (float)param[0];
            float fadeTime = (float)param[1];
            bool isLoop = (bool)param[2];

            m_BGMSource.clip = clip;
            m_BGMSource.loop = isLoop;
            m_BGMSource.volume = fadeTime > 0f ? 0f : volume;

            if (!m_IsBGMPause)
            {
                m_BGMSource.Play();

                if (fadeTime > 0f)
                {
                    m_BGMSource.DOFade(volume, fadeTime);
                }
            }
        }

        private AudioSoundPlay GetSoundSource(string path, string name, float volume)
        {
            AudioSoundPlay audioSoundPlay = null;

            if (m_SoundStack.Count > 0)
            {
                audioSoundPlay = m_SoundStack.Pop();
            }
            else
            {
                audioSoundPlay = AudioSoundPlay.Create();
                audioSoundPlay.audioSource.transform.SetParent(m_Root.transform, false);
            }

            audioSoundPlay.path = path;
            audioSoundPlay.name = name;
            audioSoundPlay.playTime = Time.time;
            audioSoundPlay.audioSource.SetActive(true);
            audioSoundPlay.audioSource.name = name;
            audioSoundPlay.audioSource.volume = volume;
            audioSoundPlay.audioSource.playOnAwake = false;
            audioSoundPlay.audioSource.loop = false;
            audioSoundPlay.audioSource.Stop();

            return audioSoundPlay;
        }

        private void PutSoundSource(AudioSoundPlay audioSoundPlay)
        {
            AudioClipPool.instance.Put(audioSoundPlay.GetResPath(), audioSoundPlay.audioSource.clip);
            audioSoundPlay.Clear();
            audioSoundPlay.audioSource.clip = null;
            audioSoundPlay.audioSource.Stop();
            audioSoundPlay.audioSource.SetActive(false);
            m_SoundStack.Push(audioSoundPlay);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            CheckAudioGroup();
            CheckSound();
        }

        private void CheckAudioGroup()
        {
            if(m_IsBGMPause)
            {
                return;
            }

            if (m_CurrPlayAudio == null && m_QueueAudioGroup.Count > 0)
            {
                m_CurrPlayAudio = m_QueueAudioGroup.Dequeue();
                m_PlayStamp = Time.time;
                InnerPlayBGM(m_CurrPlayAudio.GetPath(), m_CurrPlayAudio.volume, m_CurrPlayAudio.lerpTime, m_CurrPlayAudio.isLoop);
            }

            if (m_CurrPlayAudio != null && m_BGMSource.clip != null && !m_CurrPlayAudio.isLoop)
            {
                if (Time.time - m_PlayStamp >= m_BGMSource.clip.length)
                {
                    StopCurrent();
                }
            }
        }

        private void CheckSound()
        {
            if (m_PlayingList.Count < 1)
            {
                return;
            }

            for (int i = m_PlayingList.Count - 1; i >= 0; i--)
            {
                if (!m_PlayingList[i].audioSource.isPlaying && m_PlayingList[i].audioSource.clip != null)
                {
                    PutSoundSource(m_PlayingList[i]);
                    m_PlayingList.RemoveAt(i);
                }
            }
        }

        private void StopCurrent(bool isForceStop = false)
        {
            if (m_CurrPlayAudio != null)
            {
                AudioClipPool.instance.Put(m_CurrPlayAudio.GetPath(), m_BGMSource.clip);
                ReferencePool.Release(m_CurrPlayAudio);
                m_BGMSource.Stop();
                m_CurrPlayAudio = null;
                m_BGMSource.clip = null;
            }

            if(isForceStop)
            {
                m_QueueAudioGroup.Clear();
            }
        }

        protected override void OnShutDown()
        {
            m_QueueAudioGroup.Clear();
            m_PlayingList.Clear();
            m_SoundStack.Clear();
        }

        private bool m_IsBGMPause = false;
        private float m_PlayStamp = 0f;
        private float m_StopStamp = 0f;
        private event GameFrameWorkAction m_OnBGMFadeCompleteEvent = null;
        private AudioSource m_BGMSource = null;
        private AudioGroup m_CurrPlayAudio = null;
        private Queue<AudioGroup> m_QueueAudioGroup = null;
        private List<AudioSoundPlay> m_PlayingList = null;
        private Stack<AudioSoundPlay> m_SoundStack = null;
        private GameObject m_Root = null;
    }
}