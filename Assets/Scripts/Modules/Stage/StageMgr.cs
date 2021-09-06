using GameFrameWork;
using GameFrameWork.Camera;
using GameFrameWork.Pool;
using GameFrameWork.Resources;
using GameFrameWork.Scene;
using GameFrameWork.Sound;
using GameFrameWork.UI;
using GameFrameWork.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMgr : BaseMgr<StageMgr>
{
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

    protected override void OnAwake()
    {

    }

    public void Enter(int id, GameFrameWorkAction onEnter = null)
    {
        if (m_CurrStageData != null && m_CurrStageData.Id == id)
        {
            return;
        }

        m_OnEnterEvent = onEnter;
        m_CurrStageData = StaticConfig.StageConfig.GetData(id);

        PlayerMgr.Ins.CanContrl = false; 
        CameraMgr.Ins.EndFollow();
        SceneEntityMgr.Ins.ReleaseSceneOjbect();
        SceneMgr.Ins.LoadSceneSuccessEvent += LoadSceneSuccess;
        SceneMgr.Ins.LoadSceneAsync(m_CurrStageData.SceneName);
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
        return Util.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosX(float posX)
    {
        Vector2Int posInt = new Vector2Int((int)(posX * 100), m_CurrStageData.MovePoints[0].y);
        return Util.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosY(float posY)
    {
        Vector2Int posInt = new Vector2Int(m_CurrStageData.MovePoints[0].x, (int)(posY * 100));
        return Util.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
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
        Vector2Int[] pos = Util.PolygonRandomPoints(m_CurrStageData.MovePoints);
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
                group[i] = AudioGroup.Create(ResDefine.AUDIO_CLIP_PATH, PathUtil.FormatPath("BGM", clipName), isLoop, volume, lerpTime);
            }

            SoundMgr.Ins.PlayBGMGroup(group);
        }
        
        UIMgr.Ins.Open<MainPanel>();

        for (int i = 0; i < m_CurrStageData.TaskIDs.Length; i++)
        {
            TaskMgr.Ins.AcceptTask(m_CurrStageData.TaskIDs[i]);
        }

        SceneEntityMgr.Ins.CreateSceneBuildings(m_CurrStageData);
        PlayerMgr.Ins.InitPlayer();
        PlayerMgr.Ins.Player.SetMapPos(m_CurrStageData.InitPos);
        CameraMgr.Ins.SetFollowSize(m_CurrStageData.Width, m_CurrStageData.Height);
        CameraMgr.Ins.StartFollow();

        PlayerMgr.Ins.CanContrl = true;
        m_OnEnterEvent?.Invoke();
        m_OnEnterEvent = null;
    }


    private GameFrameWorkAction m_OnEnterEvent = null;
    private StageConfigData m_CurrStageData = null;
}