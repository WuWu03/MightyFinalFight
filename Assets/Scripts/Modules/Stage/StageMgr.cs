using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Camera;
using GameFrameWork.Event;
using GameFrameWork.GameEntity;
using GameFrameWork.Map;
using GameFrameWork.Scene;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;

public class StageMgr : BaseMgr<StageMgr>
{
    public StageConfigData currStageData
    {
        get
        {
            return m_CurrStageData;
        }
    }

    public int stageIndex
    {
        get
        {
            return m_StageIndex;
        }
    }

    public event GameFrameWorkAction onStageStartEnterEvent
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

    public event GameFrameWorkAction onStageEndEnterEvent
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


    public void StageEnter(int stageId)
    {
        StageConfigData configData = null;// StaticConfig.StageConfig.GetData(stageId);
        for (int i = 0; i < StaticConfig.StageConfig.Datas.Count; i++)
        {
            if (StaticConfig.StageConfig.Datas[i].Id == stageId)
            {
                m_StageIndex = i + 1;
                configData = StaticConfig.StageConfig.Datas[i];
                break;
            }
        }

        StageEnter(configData);
    }

    public void StageEnterNext()
    {
        StageConfigData configData = StaticConfig.StageConfig.GetDataByIndex(m_StageIndex);
        StageEnter(configData);
        m_StageIndex++;
    }

    private void StageEnter(StageConfigData configData)
    {
        if (m_CurrStageData != null && m_CurrStageData.Id == configData.Id)
        {
            return;
        }

        m_CurrStageData = configData;

        PlayerMgr.instance.canContrl = false;
        CameraMgr.instance.EndFollow();
        SceneMgr.instance.loadSceneSuccessEvent += LoadSceneSuccess;

        UIMgr.instance.Open<LoadPanel>().DOFade(0f, 1f, 0.3f, 0, () =>
        {
            m_OnStageStartEnterEvent?.Invoke();
            m_OnStageStartEnterEvent = null;

            TaskMgr.instance.GiveupTask();
            SceneEntityMgr.instance.ReleaseAll();
            EntityMgr.instance.DestoryAllUnUsedEntities();
            ReferencePool.Release();
            SceneMgr.instance.LoadSceneAsync(m_CurrStageData.SceneName);
            EventMgr.instance.Dispatch(this, GameEventArgs.Create(EventDefine.StageEnterStartEventId));
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
        return MapUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosX(float posX)
    {
        Vector2Int posInt = new Vector2Int((int)(posX * 100), m_CurrStageData.MovePoints[0].y);
        return MapUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosY(float posY)
    {
        Vector2Int posInt = new Vector2Int(m_CurrStageData.MovePoints[0].x, (int)(posY * 100));
        return MapUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public float GetRandomPosX()
    {
        return GetRandomPos(Rect.zero).x;
    }

    public float GetRandomPosY()
    {
        return GetRandomPos(Rect.zero).y;
    }

    public Vector2 GetRandomPos(Rect vision)
    {
        Vector2Int pos = MapUtil.PolygonRandomPoints(m_CurrStageData.MovePoints, vision);
        Vector2 ret = Vector2.zero;

        ret.x = (float)pos.x / 100f;
        ret.y = (float)pos.y / 100f;

        return ret;
    }

    private void LoadSceneSuccess(LoadSceneSuccessEventArgs t)
    {
        if (m_CurrStageData.BGMs.Length > 0)
        {
            AudioGroup[] groups = new AudioGroup[m_CurrStageData.BGMs.Length];

            for (int i = 0; i < m_CurrStageData.BGMs.Length; i++)
            {
                string clipName = m_CurrStageData.BGMs[i].ClipName;
                bool isLoop = m_CurrStageData.BGMs[i].IsLoop;
                float volume = m_CurrStageData.BGMs[i].Volume;
                float lerpTime = m_CurrStageData.BGMs[i].LerpTime;
                groups[i] = AudioGroup.Create(ResDefine.AudioClipPath, PathUtil.FormatPath("BGM", clipName), isLoop, volume, lerpTime);
            }

            AudioMgr.instance.PlayBGMGroup(groups, true);
        }

        if (UIMgr.instance.Get<MainPanel>() == null)
        {
            UIMgr.instance.Open<MainPanel>();
        }

        SceneEntityMgr.instance.CreateSceneBuildings(m_CurrStageData);
        PlayerMgr.instance.InitPlayer();
        PlayerMgr.instance.player.SetMapPos(m_CurrStageData.InitPos);
        CameraMgr.instance.SetFollowSize(m_CurrStageData.Width, m_CurrStageData.Height);

        UIMgr.instance.Open<LoadPanel>().DOFade(1f, 0f, 0.3f, 0, () =>
        {
            UIMgr.instance.Close<LoadPanel>();
            CameraMgr.instance.StartFollow();
            PlayerMgr.instance.canContrl = true;

            m_OnStageEndEnterEvent?.Invoke();
            m_OnStageEndEnterEvent = null;

            for (int i = 0; i < m_CurrStageData.TaskIDs.Length; i++)
            {
                TaskMgr.instance.AcceptTask(m_CurrStageData.TaskIDs[i]);
            }

            EventMgr.instance.Dispatch(this, GameEventArgs.Create(EventDefine.StageEnterEndEventId));
        });
    }

    private event GameFrameWorkAction m_OnStageStartEnterEvent = null;
    private event GameFrameWorkAction m_OnStageEndEnterEvent = null;
    private StageConfigData m_CurrStageData = null;
    private int m_StageIndex = 0;
}