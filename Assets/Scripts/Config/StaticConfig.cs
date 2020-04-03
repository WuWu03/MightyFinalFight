using UnityEngine;
using UnityEditor;
using Runtime.Config;

namespace Runtime
{
    public static class StaticConfig
    {
        public static PlayerInfo[] PlayerInfo = new PlayerInfo[1];
        public static SkillConfig SkillConfig = null;
        public static StageConfig StageConfig = null;
        public static SceneObjectConfig SceneObjectConfig = null;
        public static void InitConfig()
        {
            PlayerInfo[0] = new PlayerInfo();
            PlayerInfo[0].ID = 1;
            PlayerInfo[0].Name = "Cody";
            PlayerInfo[0].Desc = "sfsdfsdsdsfdsdsdf";
            PlayerInfo[0].ResName = "Cody";
            PlayerInfo[0].HeadIcon = "Cody";
            PlayerInfo[0].Health = 100;
            PlayerInfo[0].ATK = 10;
            PlayerInfo[0].CurrLevel = 1;
            PlayerInfo[0].CurrExp = 0;
            PlayerInfo[0].JumpForce = new Vector2(40f, 150f);
            PlayerInfo[0].AttackWait = new float[4];
            PlayerInfo[0].Skills = new int[8];
            PlayerInfo[0].AttackWait[0] = 0.3f;
            PlayerInfo[0].AttackWait[1] = 0.3f;
            PlayerInfo[0].AttackWait[2] = 0.3f;
            PlayerInfo[0].AttackWait[3] = -1f;
            PlayerInfo[0].Skills[0] = 1001;
            PlayerInfo[0].Skills[1] = 1002;
            PlayerInfo[0].Skills[2] = 1003;
            PlayerInfo[0].Skills[3] = 1004;
            PlayerInfo[0].Skills[4] = 1005;
            PlayerInfo[0].Skills[5] = 1006;
            PlayerInfo[0].Skills[6] = 1007;
            PlayerInfo[0].Skills[7] = 1008;
            SkillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>("Assets/ConfigData/SkillData.asset");
            StageConfig = AssetDatabase.LoadAssetAtPath<StageConfig>("Assets/ConfigData/StageData.asset");
            SceneObjectConfig = AssetDatabase.LoadAssetAtPath<SceneObjectConfig>("Assets/ConfigData/SceneObjectData.asset");
        }
    }
}
