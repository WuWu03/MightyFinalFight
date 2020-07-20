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
        if (CurrID.Equals(id)) return;
        m_CurrID = id;
        m_CurrStageData = StaticConfig.StageConfig.GetData(id);
        m_Width = m_CurrStageData.Width;
        m_Height = m_CurrStageData.Height;
        m_CurrAreaIndex = 0;

        CreateSceneObject();
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
        m_AreaBound.width = area.Width;
        m_AreaBound.height = area.Height;
        m_AreaBound.xMin = area.Pos.x - area.Width / 2f;
        m_AreaBound.xMax = area.Pos.x + area.Width / 2f;
        m_AreaBound.yMin = area.Pos.y - area.Height / 2f;
        m_AreaBound.yMax = area.Pos.y + area.Height / 2f;

        return m_AreaBound;
    }

    private bool IsInAreaPos2(Area area, Vector2 pos)
    {
        return IsInAreaPosX(area, pos.x) && IsInAreaPosY(area, pos.y);
    }

    private void OnLoadComplete(UnityEngine.Object obj)
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

    private void CreateSceneObject()
    {
        for (int i = 0; i < m_CurrStageData.SceneObjIDs.Length; i++)
        {
            int id = m_CurrStageData.SceneObjIDs[i];
            SceneObjectData data = StaticConfig.SceneObjectConfig.GetData(id);

            if (data == null) continue;
            switch (data.Type)
            {
                case SceneObjectData.SceneObjectType.Trag:
                    Trag trag = SceneObjectPool.Ins.Get<Trag>("Trag_" + i);
                    trag.SetTragData(data);
                    break;
                case SceneObjectData.SceneObjectType.Drop:
                    break;
                case SceneObjectData.SceneObjectType.Obstacle:
                    break;
            }
        }
    }

    private void CreateEnemy()
    {
        for (int i = 0; i < 1; i++)// m_CurrStageData.EnemyAreas[0].Enemys.Length; i++)
        {
            BaseEnemy enemy = SceneObjectPool.Ins.Get<BaseEnemy>("Monster" + i);
            StageData.Enemy enemyInfo = m_CurrStageData.EnemyAreas[0].Enemys[0];
            EnemyData enemyData = StaticConfig.EnemyConfig.GetData(enemyInfo.EnemyID);

            enemy.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, enemyData.AssetName));
            enemy.InitData(new BaseRoleData()
            {
                Health = 20,
                MaxHealth = 20,
                AttackSpeed = enemyData.AttackSpeed,
                AttackValue = 1,
                Defense = 1,
                MoveSpeed = enemyData.MoveSpeed,
            });

            enemy.AddCtrl<BaseEnemyCtrl>().InitData(new BaseRoleSkillData()
            {
                AttackIDs = enemyData.AttackIDs,
                Skills = enemyData.Skills,
                AttackWait = enemyData.AttackWait,
                AttackNextTime = enemyData.AttackNextTime,
            });

            enemy.SetObjectType(ObjectType.Monster);
            enemy.SetMapPos(enemyInfo.InitPos);
        }
    }

    private void CreateSceneItem()
    {
        SceneItemData data = StaticConfig.SceneItemConfig.GetData(1001);
        Weapon weapon = SceneObjectPool.Ins.Get<Weapon>("Weapon1");
        weapon.InitData(new ItemData()
        {
            Health = data.Value,
            MaxHealth = data.Value,
            TriggerOffest = data.TriggerOffest,
            TriggerSize = data.TriggerSize,
            Value = data.Value,
        });
        weapon.SetRes(string.Format("{0}/{1}.prefab", ResDefine.PREFAB_PATH, data.AssetName));
        weapon.SetObjectType(ObjectType.SceneItem);
        weapon.SetMapPos(new Vector2Int(-320, -60));
    }

    private int m_Width;
    private int m_Height;
    private Rect m_AreaBound = Rect.zero;
    private Vector2 m_RandomPos2 = Vector2.zero;
    private SpriteRenderer m_MapRenderer = null;
    private StageData m_CurrStageData = null;
    private int m_CurrID = 0;
    private int m_CurrAreaIndex = 1;
}