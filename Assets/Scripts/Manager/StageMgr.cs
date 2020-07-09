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

        float x = -1f;
        for (int i = 0; i < 1; i++)
        {
            x += 0.2f * (float)i;
            BaseEnemy enemy = SceneObjectPool.Ins.Get<BaseEnemy>("Monster" + i);
            enemy.SetRes(string.Format("{0}/{1}.prefab", ResDefine.MODEL_PATH, "Cody"));
            enemy.InitData(new BaseRoleData()
            {
                Health = 20,
                MaxHealth = 20,
                AttackSpeed = 0.8f,
                AttackValue = 1,
                Defense = 1,
                JumpForce = Vector2.up * 20,
                MoveSpeed = 1
            });
            enemy.SetObjectType(ObjectType.Monster);
            enemy.SetPos2(x, -0.35f);
        }

        CreateSceneObject();
        CameraMgr.Ins.EndFollow();
        string resPath = ResDefine.TEX_PATH + m_CurrStageData.AssetName;
        ResMgr.Ins.LoadAsset(resPath, OnLoadComplete, true, typeof(Sprite));
    }

    public bool IsOutArea(Vector2 pos)
    {
        if (m_CurrStageData.Areas == null || m_CurrStageData.Areas.Length < 1)
        {
            return false;
        }

        return IsInArea(m_CurrStageData.Areas[m_CurrAreaIndex], pos);
    }

    public bool CanMove(Vector2 pos)
    {
        for (int i = 0; i < m_CurrStageData.MoveArea.Length; i++)
        {
            if (IsInArea(m_CurrStageData.MoveArea[i], pos))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsInArea(Area area, Vector2 pos)
    {
        int xLeft = area.Pos.x - area.Width / 2;
        int xRigth = area.Pos.x + area.Width / 2;
        int yLeft = area.Pos.y - area.Height / 2;
        int yRigth = area.Pos.y + area.Height / 2;

        pos = pos * 100;

        if (pos.x > xLeft && pos.x < xRigth && pos.y > yLeft && pos.y < yRigth)
        {
            return true;
        }

        return false;
    }

    private void OnLoadComplete(UnityEngine.Object obj)
    {
        Sprite sprite = obj as Sprite;
        m_MapRenderer.sprite = sprite;

        PlayerMgr.Ins.Player.SetMapPos(m_CurrStageData.InitPos);
        CameraMgr.Ins.InitFollow(m_CurrStageData.Width, m_CurrStageData.Height);
        SoundMgr.Ins.PlayBGM(ResDefine.AUDIO_CLIP_PATH + "/BGM", "bgm2", true, 0.3f);
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