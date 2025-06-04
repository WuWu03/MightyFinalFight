using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork.Audio
{
    public class AudioSEInfo
    {
        public AudioSource audioSource { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public float playTime { get; set; }

        public AudioSEInfo()
        {
            audioSource = new GameObject().GetOrAddComponent<AudioSource>();
        }

        public string GetResPath()
        {
            return PathUtil.FormatPath(path, name);
        }

        public static AudioSEInfo Create()
        {
            return new AudioSEInfo();
        }

        public void Clear()
        {
            path = string.Empty;
            name = string.Empty;
            playTime = 0;
        }
    }
}