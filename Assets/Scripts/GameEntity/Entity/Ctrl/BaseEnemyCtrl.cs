using GameFrameWork.BehaviourTree;
using GameFrameWork.Input;
using UnityEngine;

public class BaseEnemyCtrl : BaseRoleCtrl
{
    protected override void OnInit()
    {
        base.OnInit();
        m_BehaviourTreeMgr = new BehaviourTreeMgr(this);
    }

    public override void SetData(BaseRoleSkillData data)
    {
        BaseEnemySkillData baseEnemySkillInfo = data as BaseEnemySkillData;
        m_BehaviourTreeMgr.InitTree(baseEnemySkillInfo.behaviourTreeIds);
        base.SetData(data);
    }

    protected override void OnStart()
    {
        base.OnStart();
        m_BehaviourTreeMgr.Start();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        m_BehaviourTreeMgr.Update(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //m_Owner.OnHurtMsg(new HurtData() { attackerDir = 1, attackerId = 10011, attackValue = 1 });
            OppositePlayer();

        }

        //Vector2 dir = Vector2.zero;
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    dir.y = 1f;
        //}

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    dir.y = -1f;
        //}

        //if (Input.GetKey(KeyCode.LeftArrow)) {
        //    dir.x = -1f;
        //}

        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    dir.x = 1f;
        //}

        //MoveData m = MoveData.Create();
        //m.dir = dir;
        //m.canChangeDir = true;
        //m_Owner.OnMoveMsg(m);

        //if(Input.GetKeyDown(KeyCode.P)) 
        //{
        //    DeploySkill(2001001);
        //}
    }

    protected override void OnRelease()
    {
        m_BehaviourTreeMgr.ShutDown();
        m_BehaviourTreeMgr = null;
        base.OnRelease();
    }

    public void OppositePlayer()
    {
        m_Owner.SetDir(PlayerMgr.instance.player.pos.x - m_Owner.pos.x > 0 ? 1f : -1f);
    }

    protected BehaviourTreeMgr m_BehaviourTreeMgr = null;
}
