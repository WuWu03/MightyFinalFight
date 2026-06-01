namespace WuWuFramework.Audio
{
    public class AudioSourceInfo : WuWuFrameworkEventArg
    {
        public float volume { get; set; }
        public float fadeTime { get; set; }
        public bool isLoop { get; set; }

        public static AudioSourceInfo Create(float volume, float fadeTime, bool isLoop)
        {
            AudioSourceInfo info = ReferencePool.Acquire<AudioSourceInfo>();
            info.volume = volume;
            info.fadeTime = fadeTime;
            info.isLoop = isLoop;
            return info;
        }

        public override void Clear()
        {
            volume = 0;
            fadeTime = 0;
            isLoop = false;
        }
    }
}