using UnityEngine;

namespace GameFrameWork
{
    public class GameFrameWorkConfig : ScriptableObject
    {
        public bool isCheckVersion = false;
        public bool isLoadFromAssetBundle = false;
        public bool isOpenLog = false;
        public string uiPrefabsPath = string.Empty;
        public string uiSpritesPath = string.Empty;
        public string configDataPath = string.Empty;
        public string versionFileName = string.Empty;
        public string assetMapFileName = string.Empty;
        public Color logColor = Color.white;
    }
}