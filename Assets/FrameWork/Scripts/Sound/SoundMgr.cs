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

        private AudioSource m_Source = null;
        private void Awake()
        {
            m_Root = new GameObject("SoundMgr");
            m_Source = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            m_QueueAudioGroup = new Queue<AudioGroup>();
            DontDestroyOnLoad(m_Root);
        }

        public void PlaySound(string path,string name,float volume = 1)
        {
            string resPath = string.Format("{0}/{1}", path, name);
            AudioClipPool.Ins.Get(resPath, (AudioClip clip) =>
            {
                m_Source.PlayOneShot(clip, volume);
                AudioClipPool.Ins.Put(resPath, clip);
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

        public void PlayBGM(string path,string name, bool isLoop,float volum = 1, float fadeTime = 0)
        {
            string resPath = string.Format("{0}/{1}", path, name);
            StopCurrent();
            m_QueueAudioGroup.Clear();
            InnerPlayBGM(resPath, volum, fadeTime, isLoop);
        }

        private void Update()
        {
            if(m_CurrPlayAudio == null && m_QueueAudioGroup.Count > 0)
            {
                m_CurrPlayAudio = m_QueueAudioGroup.Dequeue();
                m_PlayStamp = Time.time;
                InnerPlayBGM(m_CurrPlayAudio.GetPath(),
                             m_CurrPlayAudio.Volum,
                             m_CurrPlayAudio.LerpTime,
                             m_CurrPlayAudio.IsLoop);
            }

            if (m_CurrPlayAudio != null && m_Source.clip != null && !m_CurrPlayAudio.IsLoop)
            {
                if (Time.time - m_PlayStamp >= m_Source.clip.length)
                {
                    StopCurrent();
                }
            }
        }

        private void InnerPlayBGM(string path,float volum,float fadeTime,bool isLoop)
        {
            AudioClipPool.Ins.Get(path, (AudioClip clip) =>
            {
                m_Source.clip = clip;
                m_Source.loop = isLoop;
                m_Source.volume = fadeTime > 0f ? 0f : volum;
                m_Source.Play();

                if (fadeTime > 0f)
                    m_Source.DOFade(volum, fadeTime);
            });
        }

        private void StopCurrent()
        {
            if(m_CurrPlayAudio != null)
            {
                AudioClipPool.Ins.Put(m_CurrPlayAudio.GetPath(), m_Source.clip);
                m_CurrPlayAudio = null;
                m_Source.Stop();
                m_Source.clip = null;
            }
        }

        public override void ShutDown()
        {

        }

        private float m_PlayStamp = 0f;
        private AudioGroup m_CurrPlayAudio = null;
        private Queue<AudioGroup> m_QueueAudioGroup = null;
        private GameObject m_Root = null;
    }
}