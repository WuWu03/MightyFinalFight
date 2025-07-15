using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.Camera;
using GameFrameWork.Event;
using GameFrameWork.GameEntity;
using GameFrameWork.Map;
using GameFrameWork.Pool;
using GameFrameWork.Scene;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using System;
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

    protected override void OnAwake()
    {

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
            SceneMgr.instance.UnLoadScene(m_CurrStageData.assetPath);
        }

        m_CurrStageData = configData;

        PlayerMgr.instance.canContrl = false;
        CameraMgr.instance.EndFollow();  
        LoadPanelMgr.instance.DOFadeBlack(OnFadeBlackComplete);
    }

    private void OnFadeBlackComplete()
    {
        EventMgr.instance.DispatchNow(this, GameEventArgs.Create(EventDefine.StageEnterStartEvent));
        TaskMgr.instance.GiveupTask();
        SceneEntityMgr.instance.ReleaseAll();
        EntityMgr.instance.DestoryAllUnUsedEntities();
        AudioMgr.instance.ReleaseAuioClips();
        GameObjectPoolMgr.instance.CheckRelease();
        AssetsPool.instance.CheckRelease();
        ReferencePool.Release();
        GC.Collect();
        SceneMgr.instance.loadSceneSuccessEvent += LoadSceneSuccess;
        SceneMgr.instance.LoadSceneAsync(m_CurrStageData.assetPath, false);
    }

    private void LoadSceneSuccess(LoadSceneSuccessEventArgs e)
    {
        if (m_CurrStageData.BGMs.Length > 0)
        {
            BGMInfo[] bgmInfos = new BGMInfo[m_CurrStageData.BGMs.Length];

            for (int i = 0; i < m_CurrStageData.BGMs.Length; i++)
            {
                string clipName = m_CurrStageData.BGMs[i].ClipName;
                bool isLoop = m_CurrStageData.BGMs[i].IsLoop;
                float volume = m_CurrStageData.BGMs[i].Volume;
                float lerpTime = m_CurrStageData.BGMs[i].LerpTime;
                string assetPath = PathUtil.FormatPath(AssetPathDefine.AudioClipPath, clipName);
                bgmInfos[i] = BGMInfo.Create(assetPath, isLoop, volume, lerpTime);
            }

            AudioMgr.instance.PlayBGMGroup(bgmInfos, true);
        }

        UIMgr.instance.Open(UINames.MainPanel).Show();
        SceneEntityMgr.instance.CreateSceneBuildings(m_CurrStageData);
        PlayerMgr.instance.InitPlayer();
        PlayerMgr.instance.player.SetMapPos(m_CurrStageData.InitPos);
        PlayerMgr.instance.canContrl = false;
        CameraMgr.instance.SetFollowSize(m_CurrStageData.Width, m_CurrStageData.Height);
        SceneMgr.instance.AllowScene();
        LoadPanelMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
    }

    private void OnFadeWhiteComplete()
    {
        LoadPanelMgr.instance.CloseLoadPanel();
        EventMgr.instance.Dispatch(this, GameEventArgs.Create(EventDefine.StageEnterEndEvent));
        CameraMgr.instance.StartFollow();
        PlayerMgr.instance.canContrl = true;
        for (int i = 0; i < m_CurrStageData.TaskIDs.Length; i++)
        {
            TaskMgr.instance.AcceptTask(m_CurrStageData.TaskIDs[i]);
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

        ret.x = pos.x / 100f;
        ret.y = pos.y / 100f;

        return ret;
    }

    private StageConfigData m_CurrStageData = null;
    private int m_StageIndex = 0;
}