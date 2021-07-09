using GameFrameWork.Utility;
using UnityEngine;

namespace GameFrameWork.Sound
{
    public class AudioGroup :IReference
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsLoop { get; set; }
        public float Volum { get; set; }
        public float LerpTime { get; set; }

        public AudioGroup()
        {

        }

        public AudioGroup(string path, string name, bool isLoop, float volum, float lerpTime)
        {
            Path = path;
            Name = name;
            IsLoop = isLoop;
            Volum = volum;
            LerpTime = lerpTime;
        }

        public static AudioGroup Create(string path, string name, bool isLoop, float volum, float lerpTime)
        {
            AudioGroup group = ReferencePool.Acquire<AudioGroup>();
            group.Path = path;
            group.Name = name;
            group.IsLoop = isLoop;
            group.Volum = volum;
            group.LerpTime = lerpTime;
            return group;
        }

        public string GetPath()
        {
            return PathUtil.FormatPath(Path, Name);
        }

        public void Clear()
        {
            Path = string.Empty;
            Name = string.Empty;
            IsLoop = false;
            Volum = 0;
            LerpTime = 0;
        }
    }
}