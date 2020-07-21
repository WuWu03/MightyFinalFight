using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct ResDefine
{
    public const string WebUrl = "http://localhost:8081";
#if UNITY_EDITOR
    public static string ConfigDataPath = Application.dataPath + "/ConfigData";
    public static string AssetBundlePath = Application.streamingAssetsPath;
#else
    public static string ConfigDataPath = Application.persistentDataPath + "/ConfigData";
    public static string AssetBundlePath = Application.persistentDataPath;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public const string DownloadUrl = WebUrl + "/Windows/";
#elif UNITY_ANDROID
    public const string DownloadUrl = WebUrl + "/Android/";
#elif UNITY_IOS
    public const string DownloadUrl = WebUrl + "/iOS/";
#endif

    public const string PREFAB_PATH = "ArtResources/Prefabs";
    public const string EFFECT_PATH = "ArtResources/Prefabs/FX";
    public const string UI_PATH = "ArtResources/UI/Prefabs";
    public const string ICON_PATH = "ArtResources/UI/Icon";
    public const string TEX_PATH = "ArtResources/Texture/";
    public const string AUDIO_CLIP_PATH = "ArtResources/AudioClip";
}

