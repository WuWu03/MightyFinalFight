using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Sound
{
    public class AudioSoundPlay
    {
        public AudioSource Source { get; set; }
        public string ResPath { get; set; }
        public float PlayTime { get; set; }

        public AudioSoundPlay()
        {
            Source = new GameObject().GetOrAddComponent<AudioSource>();
        }

        public static AudioSoundPlay Create()
        {
            return new AudioSoundPlay();
        }

        public void Clear()
        {
            ResPath = string.Empty;
            PlayTime = 0;
        }
    }
}