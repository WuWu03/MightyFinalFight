using System.IO;
using UnityEngine;

namespace WuWuFramework.Sound
{
    public class SeInfo
    {
        public AudioSource audioSource { get; private set; }
        public string assetPath { get; set; }
        public float playTime { get; set; }

        public SeInfo(Transform parent)
        {
            audioSource = new GameObject().GetOrAddComponent<AudioSource>();
            audioSource.transform.SetParent(parent, false);
        }

        public void SetSeInfo(string assetPath, float playTime, float volume)
        {
            this.assetPath = assetPath;
            this.playTime = playTime;
            audioSource.SetActiveSelf(false);
            audioSource.name = Path.GetFileNameWithoutExtension(assetPath);
            audioSource.volume = volume;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.Stop();
        }

        public void Clear()
        {
            assetPath = string.Empty;
            playTime = 0;
            audioSource.clip = null;
            audioSource.Stop();
            audioSource.SetActiveSelf(false);
        }
    }
}