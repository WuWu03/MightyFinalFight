using FrameWork;
using FrameWork.Camera;
using FrameWork.Pool;
using FrameWork.Resources;
using FrameWork.Sound;
using FrameWork.UI;
using FrameWork.Utils;
using UnityEngine;

public class StageMgr : MonoSingleton<StageMgr>
{
    public int CurrID
    {
        get
        {
            return m_CurrID;
        }
    }

    public int Width
    {
        get
        {
            return m_Width;
        }
    }
    public int Heigth
    {
        get
        {
            return m_Height;
        }
    }

    private void Awake()
    {
        if (m_MapRenderer == null)
        {
            m_MapRenderer = new GameObject("Map").GetOrAddComponent<SpriteRenderer>();
            m_MapRenderer.transform.SetParent(transform, false);
            Utils.SetLayer(m_MapRenderer.gameObject, LayerMask.NameToLayer("Map"), true);
            DontDestroyOnLoad(m_MapRenderer.gameObject);
        }
    }

    public void Enter(int id)
    {
        if (m_CurrID == id) return;
        m_CurrID = id;
        m_CurrStageData = StaticConfig.StageConfig.GetData(id);
        m_Width = m_CurrStageData.Width;
        m_Height = m_CurrStageData.Height;
        m_CurrAreaIndex = 0;

        CreateEnemy();
        CreateSceneItem();
        CameraMgr.Ins.EndFollow();
        string resPath = ResDefine.TEX_PATH + m_CurrStageData.AssetName;
        ResMgr.Ins.LoadAsset(resPath, OnLoadComplete, true, typeof(Sprite));
    }


    public bool CanMovePosX(float posX)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPosX(m_CurrStageData.MoveArea[i], posX))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanMovePosY(float posY)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPosY(m_CurrStageData.MoveArea[i], posY))
            {
                return true;
            }
        }

        return false;
    }

    public bool CanMovePos2(Vector2 pos)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPos2(m_CurrStageData.MoveArea[i], pos))
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GetRandomPos2(Vector2 currPos)
    {
        return new Vector2(GetRandomX(currPos), GetRandomY(currPos));
    }

    public float GetRandomX(Vector2 currPos)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPos2(m_CurrStageData.MoveArea[i], currPos))
            {
                Rect bound = GetBound(m_CurrStageData.MoveArea[i]);
                float x = Random.Range(currPos.x - 1.1f, currPos.x + 1.1f);
                x = Mathf.Clamp(x, bound.xMin, bound.xMax);
                return x;
            }
        }

        return 0;
    }

    public float GetRandomY(Vector2 currPos)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPos2(m_CurrStageData.MoveArea[i], currPos))
            {
                Rect bound = GetBound(m_CurrStageData.MoveArea[i]);
                float y = Random.Range(bound.yMin / 100f + 0.1f, bound.yMax / 100f - 0.1f);
                return y;
            }
        }

        return 0;
    }
    private bool IsInAreaPosX(Area area,float posX)
    {
        Rect bound = GetBound(area);

        posX *= 100;

        if (posX > bound.xMin && posX < bound.xMax)
        {
            return true;
        }

        return false;
    }

    private bool IsInAreaPosY(Area area, float posY)
    {
        Rect bound = GetBound(area);

        posY *= 100;

        if (posY > bound.yMin && posY < bound.yMax)
        {
            return true;
        }

        return false;
    }

    private Rect GetBound(Area area)
    {
        m_AreaBound.width = area.Size.x;
        m_AreaBound.height = area.Size.y;
        m_AreaBound.xMin = area.Pos.x - area.Size.x / 2f;
        m_AreaBound.xMax = area.Pos.x + area.Size.x / 2f;
        m_AreaBound.yMin = area.Pos.y - area.Size.y / 2f;
        m_AreaBound.yMax = area.Pos.y + area.Size.y / 2f;

        return m_AreaBound;
    }

    private bool IsInAreaPos2(Area area, Vector2 pos)
    {
        return IsInAreaPosX(area, pos.x) && IsInAreaPosY(area, pos.y);
    }

    private void OnLoadComplete(Object obj)
    {
        Sprite sprite = obj as Sprite;
        m_MapRenderer.sprite = sprite;

        PlayerMgr.Ins.Player.SetMapPos(m_CurrStageData.InitPos);
        CameraMgr.Ins.InitFollow(m_CurrStageData.Width, m_CurrStageData.Height);
        SoundMgr.Ins.PlayBGM(ResDefine.AUDIO_CLIP_PATH + "/BGM", "bgm2", true, 0.2f);
        //SoundMgr.Ins.PlayBGMGroup(new SoundMgr.AudioGroup[2]
        //{
        //    new SoundMgr.AudioGroup()
        //    {
        //        Path = ResDefine.AUDIO_CLIP_PATH +"/BGM",
        //        Name = "bgm01_Start",
        //        IsLoop = false,
        //    },
        //    new SoundMgr.AudioGroup()
        //    {
        //        Path = ResDefine.AUDIO_CLIP_PATH +"/BGM",
        //        Name = "bgm01_Loop",
        //        IsLoop = true,
        //    },
        //});

        UIMgr.Ins.Open<MainPanel>();
    }

    private void CreateEnemy()
    {
        for (int i = 0; i < 1; i++)// m_CurrStageData.EnemyAreas[0].Enemys.Length; i++)
        {           
            EnemyData enemyData = StaticConfig.EnemyConfig.GetData(1001);
            StageFactory.CreateEnemy(enemyData, new Vector2Int(-320, -60));
        }
    }

    private void CreateSceneItem()
    {
        SceneItemData data = StaticConfig.SceneItemConfig.GetData(1001);
        StageFactory.CreateSceneItem(data, new Vector2Int(-320, -60));

        for (int i = 0; i < 4; i++)
        {
            data = StaticConfig.SceneItemConfig.GetData(1004 + i);
            StageFactory.CreateSceneItem(data, new Vector2Int(-300 + i * 20, -60));
        }
    }


    private int m_Width;
    private int m_Height;
    private Rect m_AreaBound = Rect.zero;
    private SpriteRenderer m_MapRenderer = null;
    private StageData m_CurrStageData = null;
    private int m_CurrID = 0;
    private int m_CurrAreaIndex = 1;
}