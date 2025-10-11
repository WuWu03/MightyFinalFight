/*
 * @Desc: Stage 模块 StageView 界面视图
 * @Date: 2021-09-08 12:24:44
 * @Author: WuWu
 */

using DragonBones;
using GameFrameWork;
using GameFrameWork.Audio;
using GameFrameWork.ConfigData;
using GameFrameWork.Event;
using GameFrameWork.Pool;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;
using UnityEngine.UI;

public class StageView : UIBaseView<StageViewComponent, StageViewSettings>
{
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
        RoleSelectConfigData roleSelectConfigData = ConfigDataSheet.roleSelectConfigDatas.GetConfigDataById(characterId);
        GameObjectPoolMgr.instance.GetFromAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, roleSelectConfigData.assetName), OnLoaded);

        AddEvent(EventDefine.StageEnterStartEvent, OnStageEnterStart);
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {
        int characterId = PlayerMgr.instance.selectRoleId;
        RoleSelectConfigData roleSelectConfig = ConfigDataSheet.roleSelectConfigDatas.GetConfigDataById(characterId);
        GameObjectPoolMgr.instance.Put(PathUtil.FormatPath(AssetPathDefine.PrefabPath, roleSelectConfig.assetName), m_Role);
        m_Role = null;
    }

    protected override void OnDestroy()
    {

    }

    private void OnLoaded(string assetPath, UnityEngine.Object obj, object arg)
    {
        m_Role = obj as GameObject;
        m_Role.transform.SetParent(component.heroPosGo.transform, false);
        m_Role.SetActiveSelf(true);
        m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = 0f;
        LoadMgr.instance.DOFadeWhite(OnFadeWhiteComplete);
    }

    private void OnFadeWhiteComplete()
    {
        int characterId = PlayerMgr.instance.selectRoleId;
        RoleSelectConfigData roleSelectConfig = ConfigDataSheet.roleSelectConfigDatas.GetConfigDataById(characterId);
        m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = roleSelectConfig.animSpeed;
        m_Role.GetComponent<UnityArmatureComponent>().animation.Play(roleSelectConfig.animName, 1);
        AudioMgr.instance.PlaySe(PathUtil.FormatPath(AssetPathDefine.AudioClipPath, roleSelectConfig.soundName));
        TimerMgr.instance.Register(roleSelectConfig.showTime, OnTimer);
    }

    private void OnTimer()
    {
        StageMgr.instance.StageEnterNext();
    }

    private void OnStageEnterStart(object sender, GameEventArgs e)
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

    private GameObject m_Role = null;
}