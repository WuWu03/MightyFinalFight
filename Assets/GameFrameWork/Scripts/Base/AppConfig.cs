using UnityEngine;

namespace GameFrameWork
{
    public class AppConfig : MonoSingleton<AppConfig>
    {
        [Header("* 游戏运行是否进行版本检查")]
        public bool checkVersion = false;

        [Header("* 是否启用更新")]
        public bool openUpdate = true;

        [Header("* 是否从AssetBundle加载资源")]
        public bool loadAB = false;

        [Header("* 是否打开日志输出")]
        public bool openLog = true;

        [Header("* 是否启用Lua脚本")]
        public bool useLua = false;

        [Header("* 是否从AssetBundle加载Lua文件")]
        public bool loadLuaAB = false;

        [Header("* Lua字节模式")]
        public bool luaByteMode = false;

        [Header("* Lua脚本路径")]
        public string luaDirectory = "Assets/Scripts/Lua";

        [Header("* UI路径")]
        public string uiDirectory = string.Empty;

        [Header("* 日志文本颜色")]
        public Color logColor = Color.white;

        [Header("* PC打包路径")]
        public string pcBuildPath = string.Empty;

        [Header("* 安卓打包路径")]
        public string androidBuildPath = string.Empty;

        [Header("* 苹果打包路径")]
        public string iosBuildPath = string.Empty;

        [Header("* 版本文件名称")]
        public string versionFileName = string.Empty;
    }
}