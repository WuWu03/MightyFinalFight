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

        PlayerMgr.Ins.CanContrl = false;
        m_OnEnterEvent = onEnter;
        CameraMgr.Ins.EndFollow();
        m_CurrStageData = StaticConfig.StageConfig.GetData(id);
        SceneMgr.Ins.LoadSceneSuccessEvent += LoadSceneSuccess;
        SceneMgr.Ins.LoadSceneAsync(m_CurrStageData.SceneName);
    }

    public bool CanMovePos(Vector2 pos)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPos(m_CurrStageData.MoveArea[i], pos))
            {
                return true;
            }
        }

        return false;
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

    public Vector2 GetRandomPos(Vector2 currPos, bool isCorrect = true)
    {
        return new Vector2(GetRandomPosX(currPos, isCorrect), GetRandomPosY(currPos, isCorrect));
    }

    public float GetRandomPosX(Vector2 currPos, bool isCorrect = true)
    {
        return GetRandomPos(currPos, isCorrect, false);
    }

    public float GetRandomPosY(Vector2 currPos, bool isCorrect = true)
    {
        return GetRandomPos(currPos, isCorrect, true);
    }

    private float GetRandomPos(Vector2 currPos, bool isCorrect, bool isY)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            bool conditoin = false;
            if (isCorrect) conditoin = IsInAreaPosX(m_CurrStageData.MoveArea[i], currPos.x);
            else conditoin = IsInAreaPos(m_CurrStageData.MoveArea[i], currPos);

            if (conditoin)
            {
                Rect bound = GetBound(m_CurrStageData.MoveArea[i]);
                float min = isY ? bound.yMin : bound.xMin;
                float max = isY ? bound.yMax : bound.xMax;
                float ret = isY ? Random.Range(min / 100f + 0.1f, max / 100f - 0.1f) : Random.Range(currPos.x - 1.1f, currPos.x + 1.1f);
                return isY ? ret : Mathf.Clamp(ret, min, max);
            }
        }

        return 0;
    }

    private bool IsInAreaPos(Rect area, Vector2 pos)
    {
        return IsInAreaPosX(area, pos.x) && IsInAreaPosY(area, pos.y);
    }

    private bool IsInAreaPosX(Rect area,float posX)
    {
        posX *= 100;
        Rect bound = GetBound(area);

        if (posX >= bound.xMin && posX <= bound.xMax)
        {
            return true;
        }

        return false;
    }

    private bool IsInAreaPosY(Rect area, float posY)
    {
        posY *= 100;
        Rect bound = GetBound(area);

        if (posY >= bound.yMin && posY <= bound.yMax)
        {
            return true;
        }

        return false;
    }

    private Rect GetBound(Rect area)
    {
        Rect bound = Rect.zero;
        bound.xMin = area.x;
        bound.xMax = area.x + area.width;
        bound.yMin = area.y - area.height;
        bound.yMax = area.position.y;

        return bound;
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