/*******************************************************/
/**2021-9-8 12:24****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DragonBones;
using GameFrameWork.Resources;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class StagePanel : BasePanel
{
	public override string panelName { get { return "StagePanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new StagePanelComponent(uiRefRoot);
	}

	protected override void OnOpen()
	{
		int stageId = StageMgr.instance.nextStageId;
		int characterId = PlayerMgr.instance.selectRoleId;

		StageConfigData stageConfigData = StaticConfig.StageConfig.GetData(stageId);
        RoleSelectData roleSelectData = DataHelper.roleSelectDatas.GetDataById(characterId);

		GetRoundTxt(0).text = stageConfigData.StageIndex.ToString();
		GameObjectPool.instance.Get(PathUtil.FormatPath(ResDefine.PrefabPath, roleSelectData.assetName), OnLoaded);

		for (int i = 1; i < 6; i++)
		{
			m_Component.imgMapGO.transform.Find("pos" + i).gameObject.SetActive(false);
		}

		m_Component.imgMapGO.transform.Find("pos" + stageConfigData.StageIndex).gameObject.SetActive(true);
	}

    private void OnLoaded(GameObject go, object[] args)
    {
		int characterId = PlayerMgr.instance.selectRoleId;
        RoleSelectData roleSelectConfigData = DataHelper.roleSelectDatas.GetDataById(characterId);

		m_Role = go;
		m_Role.transform.SetParent(m_Component.heroPosGO.transform, false);
		m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = roleSelectConfigData.animSpeed;
		m_Role.GetComponent<UnityArmatureComponent>().animation.Play(roleSelectConfigData.animName, 1);

		SoundMgr.instance.PlaySound(ResDefine.AudioClipPath, roleSelectConfigData.soundName);
		Timer.Register(roleSelectConfigData.showTime, OnTimer);
    }

	private void OnTimer()
	{
		StageMgr.instance.onStageStartEnterEvent += InnerClose;
		StageMgr.instance.StageEnterNext();
	}

    protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{
		int characterId = PlayerMgr.instance.selectRoleId;
		RoleSelectData roleSelectData = DataHelper.roleSelectDatas.GetDataById(characterId);

		GameObjectPool.instance.Put(PathUtil.FormatPath(ResDefine.PrefabPath, roleSelectData.assetName), m_Role);
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