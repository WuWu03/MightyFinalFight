using GameFrameWork.Serialize;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorConfig : BaseScriptableObject<MapEditorConfigData>
{
    public string MapPath = string.Empty;

    public MapEditorConfigData GetData(string path)
    {
        for (int i = 0; i < Datas.Count; i++)
        {
            if(Datas[i].MapPath.Equals(path))
            {
                return Datas[i];
            }
        }

        return null;
    }
}

[Serializable]
public class MapEditorConfigData : BaseConfigData
{
    [Serializable]
    public class MoveArea 
    {
        public Color Color;
        public Vector2 Point;
    }

    public string MapPath;
    public string SceneName;
    public string assetPath;
    public int StageIndex;
    public int Level;
    public Vector2 CurrPos = Vector2Int.zero;
    public Vector2 InitPos = Vector2Int.zero;
    public int Width;
    public int Height;
    public string StageColor;
    public int StageShowColor;
    public List<MoveArea> ListMovePoints;
    public List<int> ListTaskId;
    public List<StageConfigData.BGM> ListBGM;
    public List<StageConfigData.SceneBuilding> ListSceneBuilding;

    public override int CompareTo(object obj)
    {
        MapEditorConfigData data = obj as MapEditorConfigData;

        if (data.Id == this.Id)
        {
            return 0;
        }

        if (data.Id == 0)
        {
            return -1;
        }

        if (data.Id < this.Id)
        {
            return 1;
        }
        else
        {
            return -1;
        }

    }
}
