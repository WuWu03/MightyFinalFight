using UnityEditor;
using UnityEngine;

namespace GameFrameWork
{
    public class AppConfig : MonoSingleton<AppConfig>
    {
        [Header("* 游戏运行是否进行版本检查")]
        public bool CheckVersion = false;

        [Header("* 是否启用更新")]
        public bool OpenUpdate = true;

        [Header("* 是否从AssetBundle加载资源")]
        public bool LoadAB = false;

        [Header("* 是否打开日志输出")]
        public bool OpenLog = true;

        [Header("* 是否启用Lua脚本")]
        public bool UseLua = false;

        [Header("* 是否从AssetBundle加载Lua文件")]
        public bool LoadLuaAB = false;

        [Header("* Lua字节模式")]
        public bool LuaByteMode = false;

        [Header("* Lua脚本路径")]
        public string LuaDirectory = "Assets/Scripts/Lua";

        [Header("* 日志文本颜色")]
        public Color LogColor = Color.white;

        [Header("* PC打包路径")]
        public string PCBuildPath = string.Empty;

        [Header("* 安卓打包路径")]
        public string AndroidBuildPath = string.Empty;

        [Header("* 苹果打包路径")]
        public string iOSBuildPath = string.Empty;
    }
}