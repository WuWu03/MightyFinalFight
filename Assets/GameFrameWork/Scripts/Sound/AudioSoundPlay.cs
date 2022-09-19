using GameFrameWork.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Sound
{
    public class AudioSoundPlay
    {
        public AudioSource audioSource { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public float playTime { get; set; }

        public AudioSoundPlay()
        {
            audioSource = new GameObject().GetOrAddComponent<AudioSource>();
        }

        public string GetResPath()
        {
            return PathUtil.FormatPath(path, name);
        }

        public static AudioSoundPlay Create()
        {
            return new AudioSoundPlay();
        }

        public void Clear()
        {
            path = string.Empty;
            name = string.Empty;
            playTime = 0;
        }
    }
}