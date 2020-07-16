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

        for (int i = 0; i < 10;i++)// m_CurrStageData.EnemyAreas[0].Enemys.Length; i++)
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

        CreateSceneObject();
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

    public float GetRandomY(float posX)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInAreaPosX(m_CurrStageData.MoveArea[i], posX))
            {
                int yLeft = m_CurrStageData.MoveArea[i].Pos.y - m_CurrStageData.MoveArea[i].Height / 2 + 10;
                int yRigth = m_CurrStageData.MoveArea[i].Pos.y + m_CurrStageData.MoveArea[i].Height / 2 - 10;

                return Random.Range((float)yLeft / 100f, (float)yRigth / 100f);
            }
        }

        return 0;
    }
    private bool IsInAreaPosX(Area area,float posX)
    {
        int xLeft = area.Pos.x - area.Width / 2;
        int xRigth = area.Pos.x + area.Width / 2;

        posX *= 100;

        if (posX > xLeft && posX < xRigth)
        {
            return true;
        }

        return false;
    }

    private bool IsInAreaPosY(Area area, float posY)
    {
        int yLeft = area.Pos.y - area.Height / 2;
        int yRigth = area.Pos.y + area.Height / 2;

        posY *= 100;

        if (posY > yLeft && posY < yRigth)
        {
            return true;
        }

        return false;
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

    private int m_Width;
    private int m_Height;
    private SpriteRenderer m_MapRenderer = null;
    private StageData m_CurrStageData = null;
    private int m_CurrID = 0;
    private int m_CurrAreaIndex = 1;
}