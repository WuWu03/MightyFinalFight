using UnityEngine;

namespace FrameWork
{
    [System.Serializable]
    public class RuntimeEnvironment
    {
        [System.NonSerialized]
        public static RuntimeEnvironment Instance;

        [Header("* 游戏运行是否进行版本检查")]
        public bool checkVersionEditor;

        [Header("* 是否从AssetBundle加载资源")]
        public bool loadResFromAssetBundle;

        [Header("* 是否从AssetBundle加载Lua文件")]
        public bool loadLuaFromAssetBundle;
    }
}