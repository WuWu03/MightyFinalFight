using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(InputActionAsset))]
public class InputActionAssetEditor : Editor
{
    private InputActionAsset m_InputActionAsset;

    private void OnEnable()
    {
        if (target is InputActionAsset inputActionAsset)
        {
            m_InputActionAsset = inputActionAsset;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUI.enabled = true;
        if (GUILayout.Button("导出数据"))
        {
            string jsonStr = m_InputActionAsset.ToJson();
            byte[] jsonBuffer = System.Text.Encoding.UTF8.GetBytes(jsonStr);
            string fileName = m_InputActionAsset.name + ".bytes";
            string configDataPath = GameFrameWork.Editor.EditorMgr.GetGameFrameWorkConfig().configDataPath;
            string configDataFullPath = GameFrameWork.Utils.PathUtil.GetAssetFullPath(configDataPath);
            string filePath = GameFrameWork.Utils.PathUtil.FormatPath(configDataFullPath, fileName);
            GameFrameWork.Utils.FileUtil.CreateBinaryFile(filePath, jsonBuffer);
            AssetDatabase.Refresh();
        }
    }
}