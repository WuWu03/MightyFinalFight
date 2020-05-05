using UnityEngine;

namespace FrameWork
{
    [System.Serializable]
    public class RuntimeEnvironment
    {
        [System.NonSerialized]
        public static RuntimeEnvironment Instance;

        [Header("* 游戏运行是否进行版本检查")]
        public bool CheckVersion;

        [Header("* 是否从AssetBundle加载资源")]
        public bool LoadAB;

        [Header("* 是否从AssetBundle加载Lua文件")]
        public bool LoadLuaAB;

        [Header("* 是否打开日志输出")]
        public bool OpenLog;
    }
}