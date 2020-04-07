using FrameWork.Resources;
using UnityEngine;

namespace FrameWork.Sound
{
    public class SoundMgr : BaseMgr<SoundMgr>
    {
        private AudioSource m_Source = null;
        private void Awake()
        {
            m_Root = new GameObject("SoundMgr");
            m_Source = m_Root.GetOrAddComponent<AudioSource>();
            m_Root.GetOrAddComponent<AudioListener>();
            m_Root.transform.SetParent(transform, false);
            DontDestroyOnLoad(m_Root);
        }

        public void PlaySound(string name)
        {
            //string resPath = string.Format("{0}/Sound/{1}", ResDefine.AUDIO_CLIP_PATH, name);
            //ResMgr.Ins.LoadAsset(resPath, (UnityEngine.Object obj) =>
            //{
            //    m_Source.PlayOneShot(obj as AudioClip);
            //}, true, typeof(AudioClip));
        }

        public void PlayBGM(string name)
        {
            //string resPath = string.Format("{0}/BGM/{1}", ResDefine.AUDIO_CLIP_PATH, name);
            //ResMgr.Ins.LoadAsset(resPath, (UnityEngine.Object obj) =>
            //{
            //    m_Source.clip = obj as AudioClip;
            //    m_Source.loop = true;
            //    m_Source.Play();
            //}, true, typeof(AudioClip));
        }

        public override void ShutDown()
        {

        }

        private GameObject m_Root = null;
    }
}