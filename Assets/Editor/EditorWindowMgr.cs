using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class EditorWindowMgr : MonoBehaviour
{
    [MenuItem("Tools/CharacterTriggerEditor")]
    public static void OpenCharacterTriggerEditor()
    {
        EditorWindow.GetWindow<CharacterTriggerEditor>(false, "CharacterTriggerEditor", false).Show();
    }

    [MenuItem("Assets/Config/CreateSkillData")]
    public static void CreateSkillData()
    {
        FrameWork.Utils.Utils.CreateConfigData<SkillConfig, SkillData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateStageData")]
    public static void CreateStageData()
    {
        FrameWork.Utils.Utils.CreateConfigData<StageConfig, StageData>("StageData", ".asset");
    }

    [MenuItem("Assets/Config/CreateSceneObjectData")]
    public static void CreateSceneObjectData()
    {
        FrameWork.Utils.Utils.CreateConfigData<SceneObjectConfig, SceneObjectData>("SceneObjectData", ".asset");
    }

    [MenuItem("Assets/Config/CreateHeroData")]
    public static void CreatePlayerData()
    {
        FrameWork.Utils.Utils.CreateConfigData<HeroConfig, HeroData>("HeroData", ".asset");
    }
}