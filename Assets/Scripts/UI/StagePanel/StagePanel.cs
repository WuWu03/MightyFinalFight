/*******************************************************/
/**2021-9-8 12:24****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DragonBones;
using GameFrameWork.Audio;
using GameFrameWork.ConfigData;
using GameFrameWork.Pool;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : BasePanel
{
    protected override void OnInit(object[] param)
    {
        m_Component = GetPanelComponent<StagePanelComponent>();
    }

	protected override void OnOpen()
	{
		int stageIndex = StageMgr.instance.stageIndex;
		int characterId = PlayerMgr.instance.selectRoleId;

		StageConfigData stageConfigData = StaticConfig.StageConfig.GetDataByIndex(stageIndex);
        RoleSelectConfigData roleSelectConfigData = ConfigData.roleSelectConfigDatas.GetConfigDataById(characterId);

		GetRoundTxt(stageConfigData.StageShowColor).text = stageConfigData.StageIndex.ToString();
		GameObjectPoolMgr.instance.GetFromAsset(PathUtil.FormatPath(AssetPathDefine.PrefabPath, roleSelectConfigData.assetName), OnLoaded);

		for (int i = 1; i < 6; i++)
		{
			m_Component.imgMapGO.transform.Find("pos" + i).gameObject.SetActive(false);
		}

		m_Component.imgMapGO.transform.Find("pos" + stageConfigData.StageIndex).gameObject.SetActive(true);
	}

	private void OnLoaded(string assetPath, UnityEngine.Object obj, object[] args)
	{
		int characterId = PlayerMgr.instance.selectRoleId;
		RoleSelectConfigData roleSelectConfig = ConfigData.roleSelectConfigDatas.GetConfigDataById(characterId);

		m_Role = obj as GameObject;
		m_Role.transform.SetParent(m_Component.heroPosGO.transform, false);
		m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = roleSelectConfig.animSpeed;
		m_Role.GetComponent<UnityArmatureComponent>().animation.Play(roleSelectConfig.animName, 1);
		m_Role.SetActive(true);

		AudioMgr.instance.PlaySE(AssetPathDefine.AudioClipPath, roleSelectConfig.soundName);
		Timer.Register(roleSelectConfig.showTime, OnTimer);
	}

	private void OnTimer()
	{
		StageMgr.instance.onStageStartEnterEvent += CloseSelf;
		StageMgr.instance.StageEnterNext();
	}

    protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{
		int characterId = PlayerMgr.instance.selectRoleId;
		RoleSelectConfigData roleSelectConfig = ConfigData.roleSelectConfigDatas.GetConfigDataById(characterId);
		GameObjectPoolMgr.instance.Put(PathUtil.FormatPath(AssetPathDefine.PrefabPath, roleSelectConfig.assetName), m_Role);
		m_Role = null;
	}

	protected override void OnDestroy()
	{
	}

	private Text GetRoundTxt(int type)
    {
		GameObject go = null;
		m_Component.blue.SetActive(false);
		m_Component.green.SetActive(false);
		m_Component.red.SetActive(false);

		if (type == 1)
			go = m_Component.blue;
		else if (type == 2)
			go = m_Component.green;
		else
			go = m_Component.red;

		go.SetActive(true);
		return go.transform.Find("txtIndex").GetComponent<Text>();
    }

	private GameObject m_Role = null;
	private StagePanelComponent m_Component = null;
}