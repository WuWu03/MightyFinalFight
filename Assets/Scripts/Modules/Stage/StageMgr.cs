using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Pool;
using GameFrameWork.Resources;
using GameFrameWork.Sound;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;

public class StageMgr : BaseMgr<StageMgr>
{
    public int StageIndex
    {
        get
        {
            return m_StageIndex;
        }
    }
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

    protected override void OnAwake()
    {
        if (m_MapRenderer == null)
        {
            m_MapRenderer = new GameObject("Map").GetOrAddComponent<SpriteRenderer>();
            m_MapRenderer.transform.SetParent(transform, false);
            Utility.SetLayer(m_MapRenderer.gameObject, LayerMask.NameToLayer("Map"), true);
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
        m_StageIndex = m_CurrStageData.StageIndex;
        string resPath = ResDefine.TEX_PATH + m_CurrStageData.AssetName;
        ResMgr.Ins.LoadAssetAsync(resPath, OnLoadComplete, true, typeof(Sprite));
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

    public Vector2 GetRandomPos2(Vector2 currPos,bool isCorrect = true)
    {
        return new Vector2(GetRandomX(currPos, isCorrect), GetRandomY(currPos, isCorrect));
    }

    public float GetRandomX(Vector2 currPos, bool isCorrect = true)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            bool conditoin = false;
            if (isCorrect) conditoin = IsInAreaPosX(m_CurrStageData.MoveArea[i], currPos.x);
            else conditoin = IsInAreaPos2(m_CurrStageData.MoveArea[i], currPos);

            if (conditoin)
            {
                Rect bound = GetBound(m_CurrStageData.MoveArea[i]);
                float x = Random.Range(currPos.x - 1.1f, currPos.x + 1.1f);
                x = Mathf.Clamp(x, bound.xMin, bound.xMax);
                return x;
            }
        }

        return 0;
    }

    public float GetRandomY(Vector2 currPos, bool isCorrect = true)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            bool conditoin = false;
            if (isCorrect) conditoin = IsInAreaPosX(m_CurrStageData.MoveArea[i], currPos.x);
            else conditoin = IsInAreaPos2(m_CurrStageData.MoveArea[i], currPos);

            if (conditoin)
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

        if (posX >= bound.xMin && posX <= bound.xMax)
        {
            return true;
        }

        return false;
    }

    private bool IsInAreaPosY(Area area, float posY)
    {
        Rect bound = GetBound(area);

        posY *= 100;

        if (posY >= bound.yMin && posY <= bound.yMax)
        {
            return true;
        }

        return false;
    }

    private bool IsInAreaPos2(Area area, Vector2 pos)
    {
        return IsInAreaPosX(area, pos.x) && IsInAreaPosY(area, pos.y);
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

    private void OnLoadComplete(Object obj, object[] param)
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
        for (int i = 0; i < m_CurrStageData.TaskIDs.Length; i++)
        {
            TaskMgr.Ins.AcceptTask(m_CurrStageData.TaskIDs[i]);
        }

        SceneEntityMgr.Ins.CreateSceneItemTest();
        CameraMgr.Ins.EndFollow();
    }



    private int m_Width;
    private int m_Height;
    private int m_CurrID = 0;
    private int m_CurrAreaIndex = 1;
    private int m_StageIndex = 0;

    private Rect m_AreaBound = Rect.zero;
    private SpriteRenderer m_MapRenderer = null;
    private StageData m_CurrStageData = null;

}