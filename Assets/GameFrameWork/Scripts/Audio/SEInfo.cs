using UnityEngine;

namespace GameFrameWork.Audio
{
    public class SEInfo
    {
        public AudioSource audioSource { get; set; }
        public string path { get; set; }
        public float playTime { get; set; }

        public SEInfo()
        {
            audioSource = new GameObject().GetOrAddComponent<AudioSource>();
        }

        public static SEInfo Create()
        {
            return new SEInfo();
        }

        public void Clear()
        {
            path = string.Empty;
            playTime = 0;
        }
    }
}