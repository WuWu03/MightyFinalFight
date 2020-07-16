using FrameWork.Pool;
using FrameWork.Resources;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FrameWork.Sound
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
            public string GetPath()
            {
                return string.Format("{0}/{1}", Path, Name);
            }
        }

        private class AudioSoundPlay
        {
            public AudioSource Source;
            public string ResPath;
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
            PutSoundSource(GetSoundSource(null, "First", string.Empty, 0));
            DontDestroyOnLoad(m_Root);
        }

        public void PlaySound(string path, string name, float volume = 1)
        {
            string resPath = string.Format("{0}/{1}", path, name);
            AudioClipPool.Ins.Get(resPath, (AudioClip clip) =>
            {
                AudioSoundPlay audioSoundPlay = GetSoundSource(clip, name, path, volume);
                audioSoundPlay.Source.Play();
                m_PlayingList.Add(audioSoundPlay);
            });
        }

        public void PlayBGMGroup(AudioGroup[] audioGroups)
        {
            StopCurrent();

            for (int i = 0; i < audioGroups.Length; i++)
            {
                m_QueueAudioGroup.Enqueue(audioGroups[i]);
            }
        }

        public void PlayBGM(string path, string name, bool isLoop, float volum = 1, float lerpTime = 0)
        {
            StopCurrent();
            m_QueueAudioGroup.Clear();
            m_QueueAudioGroup.Enqueue(new AudioGroup() 
            {
                Path = path,
                Name = name,
                IsLoop = isLoop,
                Volum = volum,
                LerpTime = lerpTime,
            });
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
                if (!m_PlayingList[i].Source.isPlaying)
                {
                    PutSoundSource(m_PlayingList[i]);
                    m_PlayingList.RemoveAt(i);
                }
            }
        }

        private void InnerPlayBGM(string path, float volum, float fadeTime, bool isLoop)
        {
            AudioClipPool.Ins.Get(path, (AudioClip clip) =>
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

        private AudioSoundPlay GetSoundSource(AudioClip clip,string name, string resPath,float volumn)
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
                    ResPath = resPath,
                };

                audioSoundPlay.Source.transform.SetParent(m_Root.transform, false);
            }

            audioSoundPlay.Source.name = name;
            audioSoundPlay.Source.clip = clip;
            audioSoundPlay.Source.volume = volumn;
            audioSoundPlay.Source.playOnAwake = false;
            audioSoundPlay.Source.loop = false;
            audioSoundPlay.Source.SetActive(true);
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

        public override void ShutDown()
        {
            m_QueueAudioGroup.Clear();
        }

        private float m_PlayStamp = 0f;
        private AudioGroup m_CurrPlayAudio = null;
        private Queue<AudioGroup> m_QueueAudioGroup = null;
        private List<AudioSoundPlay> m_PlayingList = null;
        private Stack<AudioSoundPlay> m_SoundStack = null;
        private GameObject m_Root = null;
    }
}