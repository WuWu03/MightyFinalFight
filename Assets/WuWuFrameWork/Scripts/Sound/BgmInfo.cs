namespace WuWuFramework.Audio
{
    public class BgmInfo : WuWuFrameworkEventArg
    {
        public string assetPath { get; set; }
        public bool isLoop { get; set; }
        public float volume { get; set; }
        public float lerpTime { get; set; }

        public static BgmInfo Create(string assetPath, bool isLoop, float volume, float lerpTime)
        {
            BgmInfo bgmInfo = ReferencePool.Acquire<BgmInfo>();
            bgmInfo.assetPath = assetPath;
            bgmInfo.isLoop = isLoop;
            bgmInfo.volume = volume;
            bgmInfo.lerpTime = lerpTime;
            return bgmInfo;
        }

        public override void Clear()
        {
            assetPath = string.Empty;
            isLoop = false;
            volume = 0;
            lerpTime = 0;
        }
    }
}