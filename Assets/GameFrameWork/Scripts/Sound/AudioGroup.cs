using GameFrameWork.Utilities;
using UnityEngine;

namespace GameFrameWork.Sound
{
    public class AudioGroup :IReference
    {
        public string path { get; set; }
        public string name { get; set; }
        public bool isLoop { get; set; }
        public float volume { get; set; }
        public float lerpTime { get; set; }

        public AudioGroup()
        {

        }

        public AudioGroup(string path, string name, bool isLoop, float volume, float lerpTime)
        {
            this.path = path;
            this.name = name;
            this.isLoop = isLoop;
            this.volume = volume;
            this.lerpTime = lerpTime;
        }

        public static AudioGroup Create(string path, string name, bool isLoop, float volume, float lerpTime)
        {
            AudioGroup group = ReferencePool.Acquire<AudioGroup>();
            group.path = path;
            group.name = name;
            group.isLoop = isLoop;
            group.volume = volume;
            group.lerpTime = lerpTime;
            return group;
        }

        public string GetPath()
        {
            return PathUtil.FormatPath(path, name);
        }

        public void Clear()
        {
            path = string.Empty;
            name = string.Empty;
            isLoop = false;
            volume = 0;
            lerpTime = 0;
        }
    }
}