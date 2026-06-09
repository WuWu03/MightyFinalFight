using System;
using WuWuFramework;
using WuWuFramework.Audio;
using WuWuFramework.Event;
using WuWuFramework.Map;
using WuWuFramework.Scene;
using WuWuFramework.Utils;
using UnityEngine;

public class StageMgr : BaseMgr<StageMgr>
{
    private StageConfigData m_CurrStageData;
    private int m_StageIndex;

    public StageConfigData CurrStageData
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

    public void StageEnter(int stageId)
    {
        StageConfigData configData = null;

        for (int i = 0; i < StaticConfig.StageConfig.listDatas.Count; i++)
        {
            if (StaticConfig.StageConfig.listDatas[i].id == stageId)
            {
                m_StageIndex = i + 1;
                configData = StaticConfig.StageConfig.listDatas[i];
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
        if (m_CurrStageData != null && m_CurrStageData.id == configData.id)
        {
            return;
        }

        if (m_CurrStageData != null)
        {
            GameEntry.sceneMgr.UnLoadScene(m_CurrStageData.assetPath);
        }

        m_CurrStageData = configData;
        PlayerMgr.instance.canControl = false;
        CameraMgr.instance.cameraFollow.EndFollow();
        LoadMgr.instance.DOFadeBlack(OnFadeBlackComplete);
    }

    private void OnFadeBlackComplete()
    {
        GameEntry.eventMgr.DispatchNow(this, new StageEnterStartEvent());
        TaskMgr.instance.GiveupTask();
        SceneEntityMgr.instance.ReleaseAll();
        GameEntry.entityMgr.DestroyAllUnUsedEntities();
        GameEntry.soundMgr.ReleaseSeAudioSources();
        GameEntry.gameObjectPoolMgr.CheckRelease();
        GameEntry.resourcePoolMgr.CheckRelease();
        GameEntry.configDataMgr.RemoveAll();
        ReferencePool.ReleaseAll();
        GC.Collect();
        GameEntry.sceneMgr.loadSceneSuccessEvent += LoadSceneSuccess;
        GameEntry.sceneMgr.LoadSceneAsync(m_CurrStageData.assetPath, false);
    }

    private void LoadSceneSuccess(LoadSceneSuccessEventArgs e)
    {
        if (m_CurrStageData.BGMs.Length > 0)
        {
            BgmInfo[] bgmInfos = new BgmInfo[m_CurrStageData.BGMs.Length];

            for (int i = 0; i < m_CurrStageData.BGMs.Length; i++)
            {
                string clipName = m_CurrStageData.BGMs[i].ClipName;
                bool isLoop = m_CurrStageData.BGMs[i].IsLoop;
                float volume = m_CurrStageData.BGMs[i].Volume;
                float lerpTime = m_CurrStageData.BGMs[i].LerpTime;
                string assetPath = PathUtil.FormatPath(AssetPathDefine.AudioClipPath, clipName);
                bgmInfos[i] = BgmInfo.Create(assetPath, isLoop, volume, lerpTime);
            }

            GameEntry.soundMgr.PlayBgmGroup(bgmInfos, true);
        }

        SceneEntityMgr.instance.CreateSceneBuildings(m_CurrStageData);
        PlayerMgr.instance.InitPlayer();
        PlayerMgr.instance.player.SetMapPos(m_CurrStageData.InitPos);
        PlayerMgr.instance.canControl = false;
        CameraMgr.instance.cameraFollow.SetFollowSize(m_CurrStageData.Width, m_CurrStageData.Height);

        if (m_CurrStageData.showMainPanel)
        {
            GameEntry.uiMgr.Open<MainView>();
        }
        else
        {
            GameEntry.uiMgr.Close<MainView>();
        }

        GameEntry.sceneMgr.AllowScene();
        LoadMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
    }

    private void OnFadeWhiteComplete()
    {
        LoadMgr.instance.CloseLoadPanel();
        GameEntry.eventMgr.Dispatch(this, new StageEnterEndEvent());
        CameraMgr.instance.cameraFollow.StartFollow();
        PlayerMgr.instance.canControl = true;
        foreach (int taskID in m_CurrStageData.TaskIDs)
        {
            TaskMgr.instance.AcceptTask(taskID);
        }
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
        Vector2Int posInt = new((int)(pos.x * 100), (int)(pos.y * 100));
        return MapUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosX(float posX)
    {
        Vector2Int posInt = new((int)(posX * 100), m_CurrStageData.MovePoints[0].y);
        return MapUtil.PolygonContainsPoint(m_CurrStageData.MovePoints, posInt);
    }

    public bool CanMovePosY(float posY)
    {
        Vector2Int posInt = new(m_CurrStageData.MovePoints[0].x, (int)(posY * 100));
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

        ret.x = pos.x / 100f;
        ret.y = pos.y / 100f;

        return ret;
    }
}