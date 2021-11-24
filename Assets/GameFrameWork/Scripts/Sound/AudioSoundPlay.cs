using GameFrameWork.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Sound
{
    public class AudioSoundPlay
    {
        public AudioSource Source { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public float PlayTime { get; set; }

        public AudioSoundPlay()
        {
            Source = new GameObject().GetOrAddComponent<AudioSource>();
        }

        public string GetResPath()
        {
            return PathUtil.FormatPath(Path, Name);
        }

        public static AudioSoundPlay Create()
        {
            return new AudioSoundPlay();
        }

        public void Clear()
        {
            Path = string.Empty;
            Name = string.Empty;
            PlayTime = 0;
        }
    }
}