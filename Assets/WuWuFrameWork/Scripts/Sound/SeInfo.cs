using UnityEngine;

namespace WuWuFramework.Audio
{
    public class SeInfo
    {
        public AudioSource audioSource { get; private set; }
        public string path { get; set; }
        public float playTime { get; set; }

        public SeInfo()
        {
            audioSource = new GameObject().GetOrAddComponent<AudioSource>();
        }

        public static SeInfo Create()
        {
            return new SeInfo();
        }

        public void Clear()
        {
            path = string.Empty;
            playTime = 0;
        }
    }
}