using WuWuFramework.Serialize;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorConfig : BaseScriptableObject<MapEditorConfigData>
{
    public string mapTexturesPath = string.Empty;

    public MapEditorConfigData GetData(string path)
    {
        for (int i = 0; i < listDatas.Count; i++)
        {
            if(listDatas[i].mapPath.Equals(path))
            {
                return listDatas[i];
            }
        }

        return null;
    }
}

[Serializable]
public class MapEditorConfigData : BaseScriptableConfigData
{
    [Serializable]
    public class MoveArea 
    {
        public Color color;
        public Vector2 point;
    }

    public string mapPath;
    public string sceneName;
    public string assetPath;
    public int stageIndex;
    public int level;
    public bool showMainPanel;
    public Vector2 currPos = Vector2Int.zero;
    public Vector2 initPos = Vector2Int.zero;
    public int width;
    public int height;
    public string stageColor;
    public int stageShowColor;
    public List<MoveArea> listMovePoints;
    public List<int> listTaskIds;
    public List<StageConfigData.BGM> listBGMs;
    public List<StageConfigData.SceneBuilding> listSceneBuildings;

    public override int CompareTo(object obj)
    {
        MapEditorConfigData data = obj as MapEditorConfigData;

        if (data.id == this.id)
        {
            return 0;
        }

        if (data.id == 0)
        {
            return -1;
        }

        if (data.id < this.id)
        {
            return 1;
        }
        else
        {
            return -1;
        }

    }
}
