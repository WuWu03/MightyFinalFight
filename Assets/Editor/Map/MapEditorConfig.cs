using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameWork.Serialize;
using System;

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
        public Rect Rect;
        public Rect RealRect;
    }

    public string MapPath;
    public string SceneName;
    public Vector2 CurrPos = Vector2.zero;
    public Vector2 InitPos = Vector2.zero;
    public int Width;
    public int Height;
  
    public List<MoveArea> ListMoveArea;
    public List<int> ListTaskId;
    public List<StageConfigData.BGM> ListBGM;
    public List<StageConfigData.SceneObj> ListSceneObj;
}
