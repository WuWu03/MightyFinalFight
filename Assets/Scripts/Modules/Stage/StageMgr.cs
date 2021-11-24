using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Pool;
using GameFrameWork.Resources;
using GameFrameWork.Scene;
using GameFrameWork.Sound;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMgr : BaseMgr<StageMgr>
{
    public int NextStageId
    {
        get
        {
            return m_NextStageId;
        }
        set
        {
            m_NextStageId = value;
        }
    }

    public int StageIndex
    {
        get
        {
            return m_CurrStageData.StageIndex;
        }
    }

    public int StageLevel
    {
        get
        {
            return m_CurrStageData.Level;
        }
    }

    public int Width
    {
        get
        {
            return m_CurrStageData.Width;
        }
    }

    public int Heigth
    {
        get
        {
            return m_CurrStageData.Height;
        }
    }

    public event GameFrameWorkAction OnStageStartEnterEvent
    {
        add
        {
            m_OnStageStartEnterEvent += value;
        }
        remove
        {
            m_OnStageStartEnterEvent -= value;
        }
    }

    public event GameFrameWorkAction OnStageEndEnterEvent
    {
        add
        {
            m_OnStageEndEnterEvent += value;
        }
        remove
        {
            m_OnStageEndEnterEvent -= value;
        }
    }

    protected override void OnAwake()
    {

    }

    public void StageEnterNext()
    {
        StageEnter(m_NextStageId);
    }

    public void StageEnter(int id)
    {
        if (m_CurrStageData != null && m_CurrStageData.Id == id)
        {
            return;
        }

        m_CurrStageData = StaticConfig.StageConfig.GetData(id);

        PlayerMgr.Ins.CanContrl = false; 
        CameraMgr.Ins.EndFollow();
        SceneMgr.Ins.LoadSceneSuccessEvent += LoadSceneSuccess;

        UIMgr.Ins.Open<LoadPanel>().DOFade(0f, 1f, 0.3f, 0, () =>
        {
            m_OnStageStartEnterEvent?.Invoke();
            m_OnStageStartEnterEvent = null;
            SceneEntityMgr.Ins.ReleaseSceneBuildings();
            SceneMgr.Ins.LoadSceneAsync(m_CurrStageData.SceneName);
        });  
    }

    public Rect GetMoveArea()
    {
        int length = m_CurrStageData.MovePoints.Length;
        int index = length / 2 + length % 2;

        Vector2Int pos1 = m_CurrStageData.MovePoints[0];
        Vector2Int pos2 = m_CurrStageData.MovePoints[index];

        Rect bound = Rect.zero;
        bound.xMin = pos1.x / 100f;
        bound.xMax = pos2.x / 100f;
        bound.yMin = pos2.y / 100f;
        bound.yMax = pos1.y / 100f;
        return bound;
    }

    public bool CanMove(Vector2 pos)
    {
        Vector2Int posInt = new Vector2Int((int)(pos.x * 100), (int)(pos.y * 100));
        return CommonUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosX(float posX)
    {
        Vector2Int posInt = new Vector2Int((int)(posX * 100), m_CurrStageData.MovePoints[0].y);
        return CommonUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosY(float posY)
    {
        Vector2Int posInt = new Vector2Int(m_CurrStageData.MovePoints[0].x, (int)(posY * 100));
        return CommonUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public float GetRandomPosX()
    {
        return GetRandomPos().x;
    }

    public float GetRandomPosY()
    {
        return GetRandomPos().y;
    }

    public Vector2 GetRandomPos()
    {
        Vector2Int[] pos = CommonUtil.PolygonRandomPoints(m_CurrStageData.MovePoints);
        Vector2 ret = Vector2.zero;

        if(pos.Length >0)
        {
            ret.x = (float)pos[0].x / 100f;
            ret.y = (float)pos[0].y / 100f;
        }

        return ret;
    }

    private void LoadSceneSuccess(LoadSceneSuccessEventArgs t)
    {
        if (m_CurrStageData.BGMs.Length > 0)
        {
            AudioGroup[] group = new AudioGroup[m_CurrStageData.BGMs.Length];

            for (int i = 0; i < m_CurrStageData.BGMs.Length; i++)
            {
                string clipName = m_CurrStageData.BGMs[i].ClipName;
                bool isLoop = m_CurrStageData.BGMs[i].IsLoop;
                float volume = m_CurrStageData.BGMs[i].Volume;
                float lerpTime = m_CurrStageData.BGMs[i].LerpTime;
                group[i] = AudioGroup.Create(ResDefine.AudioClipPath, PathUtil.FormatPath("BGM", clipName), isLoop, volume, lerpTime);
            }

            SoundMgr.Ins.PlayBGMGroup(group, true);
        }

        if (!UIMgr.Ins.IsPanelOpen<MainPanel>())
        {
            UIMgr.Ins.Open<MainPanel>();
        }

        SceneEntityMgr.Ins.CreateSceneBuildings(m_CurrStageData);
        PlayerMgr.Ins.InitPlayer();
        PlayerMgr.Ins.Player.SetMapPos(m_CurrStageData.InitPos);
        CameraMgr.Ins.SetFollowSize(m_CurrStageData.Width, m_CurrStageData.Height);

        UIMgr.Ins.GetPanel<LoadPanel>().DOFade(1f, 0f, 0.3f, 0, () =>
        {
            UIMgr.Ins.Close<LoadPanel>();
            CameraMgr.Ins.StartFollow();
            PlayerMgr.Ins.CanContrl = true;
            m_OnStageEndEnterEvent?.Invoke();
            m_OnStageEndEnterEvent = null;

            for (int i = 0; i < m_CurrStageData.TaskIDs.Length; i++)
            {
                TaskMgr.Ins.AcceptTask(m_CurrStageData.TaskIDs[i]);
            }
        });
    }

    private event GameFrameWorkAction m_OnStageStartEnterEvent = null;
    private event GameFrameWorkAction m_OnStageEndEnterEvent = null;
    private StageConfigData m_CurrStageData = null;
    private int m_NextStageId = 0;
}