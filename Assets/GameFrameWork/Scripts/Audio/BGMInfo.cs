namespace GameFrameWork.Audio
{
    public class BGMInfo : IReference
    {
        public string assetPath { get; set; }
        public bool isLoop { get; set; }
        public float volume { get; set; }
        public float lerpTime { get; set; }

        public static BGMInfo Create(string assetPath, bool isLoop, float volume, float lerpTime)
        {
            BGMInfo bgmInfo = ReferencePool.Acquire<BGMInfo>();
            bgmInfo.assetPath = assetPath;
            bgmInfo.isLoop = isLoop;
            bgmInfo.volume = volume;
            bgmInfo.lerpTime = lerpTime;
            return bgmInfo;
        }

        public void Clear()
        {
            assetPath = string.Empty;
            isLoop = false;
            volume = 0;
            lerpTime = 0;
        }
    }
}