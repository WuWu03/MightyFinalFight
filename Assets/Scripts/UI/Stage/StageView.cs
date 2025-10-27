/*
 * @Desc: Stage 模块 StageView 界面视图
 * @Date: 2021-09-08 12:24:44
 * @Author: WuWu
 */

using DragonBones;
using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Event;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;
using UnityEngine.UI;

public class StageView : UIBaseView<StageViewComponent, StageViewSettings>
{
    private GameObject m_Role;
    private RoleSelectConfigData m_RoleSelectConfig;
    protected override void OnOpen(object arg)
    {
        
    }

    protected override void OnShow(object arg)
    {
        int stageIndex = StageMgr.instance.stageIndex;
        StageConfigData stageConfigData = StaticConfig.StageConfig.GetDataByIndex(stageIndex);
        GetRoundTxt(stageConfigData.StageShowColor).text = stageConfigData.StageIndex.ToString();

        for (int i = 1; i < 6; i++)
        {
            component.imgMapGo.transform.Find("pos" + i).gameObject.SetActiveSelf(false);
        }

        component.imgMapGo.transform.Find("pos" + stageConfigData.StageIndex).gameObject.SetActiveSelf(true);
        int characterId = PlayerMgr.instance.selectRoleId;
        m_RoleSelectConfig = GameEntry.configDataMgr.Get<RoleSelectConfigData>().GetConfigDataById(characterId);
        GameEntry.gameObjectPoolMgr.GetFromAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, m_RoleSelectConfig.assetName), OnLoaded);
        AddEvent(EventId.StageEnterStartEvent, OnStageEnterStart);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        GameEntry.gameObjectPoolMgr.Put(PathUtil.FormatPath(AssetPathDefine.PrefabPath, m_RoleSelectConfig.assetName), m_Role);
        m_Role = null;
        m_RoleSelectConfig = null;
    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {

    }

    private void OnLoaded(string assetPath, Object obj, object arg)
    {
        if (obj is GameObject roleGo)
        {
            m_Role = roleGo;
            m_Role.transform.SetParent(component.heroPosGo.transform, false);
            m_Role.SetActiveSelf(true);
            m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = 0f;
            LoadMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
        }
    }

    private void OnFadeWhiteComplete()
    {
        m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = m_RoleSelectConfig.animSpeed;
        m_Role.GetComponent<UnityArmatureComponent>().animation.Play(m_RoleSelectConfig.animName, 1);
        GameEntry.soundMgr.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, m_RoleSelectConfig.soundName));
        GameEntry.timerMgr.Register(m_RoleSelectConfig.showTime, OnTimer);
    }

    private void OnTimer()
    {
        StageMgr.instance.StageEnterNext();
    }

    private void OnStageEnterStart(object sender, GameEventArg e)
    {
        CloseSelf();
    }

    private Text GetRoundTxt(int type)
    {
        GameObject go;
        component.blueGo.SetActiveSelf(false);
        component.redGo.SetActiveSelf(false);
        component.greenGo.SetActiveSelf(false);
        
        if (type == 1)
            go = component.blueGo;
        else if (type == 2)
            go = component.redGo;
        else
            go = component.greenGo;

        go.SetActiveSelf(true);
        return go.transform.Find("txtIndex").GetComponent<Text>();
    }
}