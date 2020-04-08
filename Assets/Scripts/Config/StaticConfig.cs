using UnityEngine;
using UnityEditor;
using Runtime.Config;

namespace Runtime
{
    public static class StaticConfig
    {
        public static HeroConfig HeroConfig = null;
        public static SkillConfig SkillConfig = null;
        public static StageConfig StageConfig = null;
        public static SceneObjectConfig SceneObjectConfig = null;
        public static void InitConfig()
        {
            HeroConfig = AssetDatabase.LoadAssetAtPath<HeroConfig>("Assets/ConfigData/HeroData.asset");
            SkillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>("Assets/ConfigData/SkillData.asset");
            StageConfig = AssetDatabase.LoadAssetAtPath<StageConfig>("Assets/ConfigData/StageData.asset");
            SceneObjectConfig = AssetDatabase.LoadAssetAtPath<SceneObjectConfig>("Assets/ConfigData/SceneObjectData.asset");
        }
    }
}
