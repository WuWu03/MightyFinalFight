/*******************************************************/
/**2021-9-8 12:24****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DragonBones;
using GameFrameWork.Audio;
using GameFrameWork.Pool;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : BasePanel
{
	public override string panelName { get { return "StagePanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Normal; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer3; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new StagePanelComponent(m_UIRefRoot);
	}

	protected override void OnOpen()
	{
		int stageIndex = StageMgr.instance.stageIndex;
		int characterId = PlayerMgr.instance.selectRoleId;

		StageConfigData stageConfigData = StaticConfig.StageConfig.GetDataByIndex(stageIndex);
        RoleSelectConfigData roleSelectConfigData = ConfigDataHelper.roleSelectConfigDatas.GetConfigDataById(characterId);

		GetRoundTxt(stageConfigData.StageShowColor).text = stageConfigData.StageIndex.ToString();
		GameObjectPool.instance.GetFromAsset(PathUtil.FormatPath(ResDefine.PrefabPath, roleSelectConfigData.assetName), OnLoaded);

		for (int i = 1; i < 6; i++)
		{
			m_Component.imgMapGO.transform.Find("pos" + i).gameObject.SetActive(false);
		}

		m_Component.imgMapGO.transform.Find("pos" + stageConfigData.StageIndex).gameObject.SetActive(true);
	}

	private void OnLoaded(string assetPath, UnityEngine.Object obj, object[] args)
	{
		int characterId = PlayerMgr.instance.selectRoleId;
		RoleSelectConfigData roleSelectConfig = ConfigDataHelper.roleSelectConfigDatas.GetConfigDataById(characterId);

		m_Role = obj as GameObject;
		m_Role.transform.SetParent(m_Component.heroPosGO.transform, false);
		m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = roleSelectConfig.animSpeed;
		m_Role.GetComponent<UnityArmatureComponent>().animation.Play(roleSelectConfig.animName, 1);
		m_Role.SetActive(true);

		AudioMgr.instance.PlaySE(ResDefine.AudioClipPath, roleSelectConfig.soundName);
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
		RoleSelectConfigData roleSelectConfig = ConfigDataHelper.roleSelectConfigDatas.GetConfigDataById(characterId);
		GameObjectPool.instance.Put(PathUtil.FormatPath(ResDefine.PrefabPath, roleSelectConfig.assetName), m_Role);
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