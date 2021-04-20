using GameFrameWork.Pool;
using GameFrameWork.Resources;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameFrameWork.Sound
{
    public class SoundMgr : BaseMgr<SoundMgr>
    {
        public class AudioGroup
        {
            public string Path;
            public string Name;
            public bool IsLoop;
            public float Volum = 1f;
            public float LerpTime = 0f;
            
            public AudioGroup(string path,string name,bool isLoop,float volum,float lerpTime)
            {
                Path = path;
                Name = name;
                IsLoop = isLoop;
                Volum = volum;
                LerpTime = lerpTime;
            }

            public string GetPath()
            {
                return string.Format("{0}/{1}", Path, Name);
            }

            public static string GetPath(string path, string name)
            {
                return string.Format("{0}/{1}", path, name);
            }
        }

        private class AudioSoundPlay
        {
            public AudioSource Source;
            public string ResPath;
            public float PlayTime;
        }

        private AudioSource m_BGMSource = null;
        private void Awake()
        {
            m_Root = new GameObject("SoundMgr");
            m_BGMSource = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            m_QueueAudioGroup = new Queue<AudioGroup>();
            m_PlayingList = new List<AudioSoundPlay>();
            m_SoundStack = new Stack<AudioSoundPlay>();
            PutSoundSource(GetSoundSource("First", string.Empty, 0));
            DontDestroyOnLoad(m_Root);
        }

        public void PlaySound(string path, string name, float volume = 1)
        {
            for (int i = 0; i < m_PlayingList.Count; i++)
            {
                string soundPath = m_PlayingList[i].ResPath;
                string soundName = m_PlayingList[i].Source.name;
                float process = Time.time - m_PlayingList[i].PlayTime;
                if (soundPath.Equals(path) && soundName.Equals(name) && process <= 0.05f)
                {
                    return;
                }
            }

            AudioSoundPlay audioSoundPlay = GetSoundSource(name, path, volume);
            m_PlayingList.Add(audioSoundPlay);

            string resPath = string.Format("{0}/{1}", path, name);
            AudioClipPool.Ins.Get(resPath, (AudioClip clip,object[] param) =>
            {
                audioSoundPlay.PlayTime = Time.time;
                audioSoundPlay.Source.clip = clip;
                audioSoundPlay.Source.Play();
            });
        }

        public void PlayBGMGroup(AudioGroup[] audioGroups,bool forceReplay = false)
        {
            if(!forceReplay)
            {
                bool isAllInPlaying = true;

                for (int i = 0; i < audioGroups.Length; i++)
                {
                    if(!IsBGMPlaying(audioGroups[i].GetPath()))
                    {
                        isAllInPlaying = false;
                        break;
                    }
                }

                if (isAllInPlaying) return;
            }

            StopCurrent();

            for (int i = 0; i < audioGroups.Length; i++)
            {
                m_QueueAudioGroup.Enqueue(audioGroups[i]);
            }
        }

        public void PlayBGM(string path, string name, bool isLoop, float volum = 1, float lerpTime = 0,bool isForceReplay = false)
        {
            if (!isForceReplay && IsBGMPlaying(AudioGroup.GetPath(path, name))) return;
            StopCurrent();
            m_QueueAudioGroup.Clear();
            m_QueueAudioGroup.Enqueue(new AudioGroup(path, name, isLoop, volum, lerpTime));
        }

        public bool IsBGMPlaying(string fullName)
        {
            if (m_CurrPlayAudio != null && m_CurrPlayAudio.GetPath().Equals(fullName)) return true;
            foreach(AudioGroup group in m_QueueAudioGroup)
            {
                if(group.GetPath().Equals(fullName)) return true;
            }
            return false;
        }

        private void Update()
        {
            CheckAudioGroup();
            CheckSound();
        }

        private void CheckAudioGroup()
        {
            if (m_CurrPlayAudio == null && m_QueueAudioGroup.Count > 0)
            {
                m_CurrPlayAudio = m_QueueAudioGroup.Dequeue();
                m_PlayStamp = Time.time;
                InnerPlayBGM(m_CurrPlayAudio.GetPath(),
                             m_CurrPlayAudio.Volum,
                             m_CurrPlayAudio.LerpTime,
                             m_CurrPlayAudio.IsLoop);
            }

            if (m_CurrPlayAudio != null && m_BGMSource.clip != null && !m_CurrPlayAudio.IsLoop)
            {
                if (Time.time - m_PlayStamp >= m_BGMSource.clip.length)
                {
                    StopCurrent();
                }
            }
        }

        private void CheckSound()
        {
            if (m_PlayingList.Count < 1) return;

            for (int i = m_PlayingList.Count - 1; i >= 0; i--)
            {
                if (!m_PlayingList[i].Source.isPlaying && m_PlayingList[i].Source.clip != null)
                {
                    PutSoundSource(m_PlayingList[i]);
                    m_PlayingList.RemoveAt(i);
                }
            }
        }

        private void InnerPlayBGM(string path, float volum, float fadeTime, bool isLoop)
        {
            AudioClipPool.Ins.Get(path, (AudioClip clip,object[] param) =>
            {
                m_BGMSource.clip = clip;
                m_BGMSource.loop = isLoop;
                m_BGMSource.volume = fadeTime > 0f ? 0f : volum;
                m_BGMSource.Play();

                if (fadeTime > 0f)
                    m_BGMSource.DOFade(volum, fadeTime);
            });
        }

        private void StopCurrent()
        {
            if (m_CurrPlayAudio != null)
            {
                AudioClipPool.Ins.Put(m_CurrPlayAudio.GetPath(), m_BGMSource.clip);
                m_CurrPlayAudio = null;
                m_BGMSource.Stop();
                m_BGMSource.clip = null;
            }
        }

        private AudioSoundPlay GetSoundSource(string name, string resPath,float volume)
        {
            AudioSoundPlay audioSoundPlay = null;
            if (m_SoundStack.Count > 0)
            {
                audioSoundPlay = m_SoundStack.Pop();
            }
            else
            {
                audioSoundPlay = new AudioSoundPlay()
                {
                    Source = new GameObject().GetOrAddComponent<AudioSource>(),
                };

                audioSoundPlay.Source.transform.SetParent(m_Root.transform, false);
            }

            audioSoundPlay.ResPath = resPath;
            audioSoundPlay.PlayTime = Time.time;
            audioSoundPlay.Source.SetActive(true);
            audioSoundPlay.Source.name = name;
            audioSoundPlay.Source.volume = volume;
            audioSoundPlay.Source.playOnAwake = false;
            audioSoundPlay.Source.loop = false;
            audioSoundPlay.Source.Stop();
            return audioSoundPlay;
        }

        private void PutSoundSource(AudioSoundPlay audioSoundPlay)
        {
            AudioClipPool.Ins.Put(audioSoundPlay.ResPath, audioSoundPlay.Source.clip);
            audioSoundPlay.Source.clip = null;
            audioSoundPlay.Source.Stop();
            audioSoundPlay.Source.SetActive(false);
            m_SoundStack.Push(audioSoundPlay);
        }

        protected override void OnShutDown()
        {
            m_QueueAudioGroup.Clear();
            m_PlayingList.Clear();
            m_SoundStack.Clear();
        }

        private float m_PlayStamp = 0f;
        private AudioGroup m_CurrPlayAudio = null;
        private Queue<AudioGroup> m_QueueAudioGroup = null;
        private List<AudioSoundPlay> m_PlayingList = null;
        private Stack<AudioSoundPlay> m_SoundStack = null;
        private GameObject m_Root = null;
    }
}