/*******************************************************/
/**2021-9-8 12:24****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Pool;
using GameFrameWork.Utilities;
using System;
using DragonBones;
using GameFrameWork.Sound;
using GameFrameWork.Timer;
using GameFrameWork.Scene;

public class StagePanel : BasePanel
{
	public override string PanelName { get { return "StagePanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Pop; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.FirstLevel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Always; } }

	protected override void OnInit(object[] param)
	{
		m_Component = new StagePanelComponent(UIRefRoot);
	}

	protected override void OnOpen()
	{
		int stageId = StageMgr.Ins.NextStageId;
		int characterId = PlayerMgr.Ins.SelectCharacterId;

		StageConfigData stageConfigData = StaticConfig.StageConfig.GetData(stageId);
		RoleSelectConfigData roleSelectConfigData = StaticConfig.RoleSelectConfig.GetData(characterId);

		GetRoundTxt(0).text = stageConfigData.StageIndex.ToString();
		GameObjectPool.Ins.Get(PathUtil.FormatPath(ResDefine.PrefabPath, roleSelectConfigData.Asset), OnLoaded);

		for (int i = 1; i < 6; i++)
		{
			m_Component.ImgMapGO.transform.Find("Pos" + i).gameObject.SetActive(false);
		}

		m_Component.ImgMapGO.transform.Find("Pos" + stageConfigData.StageIndex).gameObject.SetActive(true);
	}

    private void OnLoaded(GameObject go, object[] args)
    {
		int characterId = PlayerMgr.Ins.SelectCharacterId;
		RoleSelectConfigData roleSelectConfigData = StaticConfig.RoleSelectConfig.GetData(characterId);

		m_Role = go;
		m_Role.transform.SetParent(m_Component.HeroPosGO.transform, false);
		m_Role.GetComponent<UnityArmatureComponent>().animation.timeScale = roleSelectConfigData.AnimSpeed;
		m_Role.GetComponent<UnityArmatureComponent>().animation.Play(roleSelectConfigData.Anim, 1);
	
		SoundMgr.Ins.PlaySound(ResDefine.AudioClipPath, roleSelectConfigData.Sound);
		Timer.Register(roleSelectConfigData.ShowTime, OnTimer);
    }

	private void OnTimer()
	{
		StageMgr.Ins.OnStageStartEnterEvent += InnerClose;
		StageMgr.Ins.StageEnterNext();
	}

    protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{
		int characterId = PlayerMgr.Ins.SelectCharacterId;
		RoleSelectConfigData roleSelectConfigData = StaticConfig.RoleSelectConfig.GetData(characterId);

		GameObjectPool.Ins.Put(PathUtil.FormatPath(ResDefine.PrefabPath, roleSelectConfigData.Asset), m_Role);
		m_Role = null;
	}

	protected override void OnDestroy()
	{
	}

	private Text GetRoundTxt(int type)
    {
		GameObject go = null;
		m_Component.Blue.SetActive(false);
		m_Component.Green.SetActive(false);
		m_Component.Red.SetActive(false);

		if (type == 1)
			go = m_Component.Blue;
		else if (type == 2)
			go = m_Component.Green;
		else
			go = m_Component.Red;

		go.SetActive(true);
		return go.transform.Find("txtIndex").GetComponent<Text>();
    }

	private GameObject m_Role = null;
	private StagePanelComponent m_Component = null;
}