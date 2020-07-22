/*******************************************************/
/**2020-4-14 20:34**************************************/
/**Create By GQY****************************************/
/*******************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using DG.Tweening;
using System;
using System.Security.Principal;

public class MainPanelCtrl:BasePanelCtrl
{
	protected override void OnInit(object[] param)
	{
		m_Panel = Panel as MainPanel;
	}

	protected override void OnLoaded()
	{
		m_Panel.LevelListGroupView.Init(m_Panel.LevelList, m_Panel.ItemGO, 5);
	}

	protected override BasePanel GetPanel()
	{
		return new MainPanel();
	}

	protected override void OnOpen()
	{
		m_Panel.LevelListGroupView.OnItemUpdate = OnItemUpdate;
		m_Panel.LevelListGroupView.Update(5);
		SetPlayerExp(PlayerMgr.Ins.EXP, PlayerMgr.Ins.LevelData.EXP);
		SetRound(StageMgr.Ins.StageIndex);
		SetPlayerLife(PlayerMgr.Ins.Life);
		SetPlayerHP(PlayerMgr.Ins.LevelData.Health, PlayerMgr.Ins.LevelData.Health, PlayerMgr.Ins.LevelData.HPBarWidth);
	}

	private void OnItemUpdate(MainPanel.LevelListItem obj)
	{
		int stageIndex = StageMgr.Ins.StageIndex;
		int playerLevel = PlayerMgr.Ins.Level;
		obj.ImgLevel1.gameObject.SetActive(stageIndex == 1 && playerLevel >= obj.Index);
		obj.ImgLevel2.gameObject.SetActive(stageIndex == 2 && playerLevel >= obj.Index);
		obj.ImgLevel3.gameObject.SetActive(stageIndex == 3 && playerLevel >= obj.Index);
		obj.ImgLevel4.gameObject.SetActive(stageIndex == 4 && playerLevel >= obj.Index);
		obj.ImgLevel5.gameObject.SetActive(stageIndex == 5 && playerLevel >= obj.Index);
	}

	protected override void OnUpdate()
	{
		if (m_EnemyHpBarHideTimer > 0 && Time.time - m_EnemyHpBarHideTimer >= ENEMY_HP_BAR_HIDE)
		{
			m_Panel.EnemyHpBar.gameObject.SetActive(false);
			m_EnemyHpBarHideTimer = -1;
		}
	}

	protected override void OnClose()
	{
	}

	protected override void OnDestroy()
	{
	}

	public void SetPlayerHP(int value,int max,float width = 0f)
	{	
		if (width != 0)
			m_Panel.PlayerHpBar.GetComponent<LayoutElement>().preferredWidth = width;	
		m_Panel.PlayerHpBar.maxValue = max;
		m_Panel.PlayerHpBar.value = value;
	}

	public void SetEnemyHP(int value, int max,float width)
	{
		if (m_IsEnemyHpBarAnim) return;

		m_Panel.EnemyHpBar.value = value;
		m_Panel.EnemyHpBar.maxValue = max;
		m_Panel.EnemyHpBar.gameObject.SetActive(true);
		m_Panel.EnemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		Image image = m_Panel.EnemyHpBar.GetComponent<Image>();
		image.DOFade(1, 0);

		if (value == 0)
		{
			m_EnemyHpBarHideTimer = -1;
			m_IsEnemyHpBarAnim = true;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(image.DOFade(0, 0.2f));
			sequence.Append(image.DOFade(1, 0.2f));
			sequence.Append(image.DOFade(0, 0.2f));
			sequence.Append(image.DOFade(1, 0.2f));
			sequence.Append(image.DOFade(0, 0.2f));
			sequence.Append(image.DOFade(1, 0.2f));
			sequence.Append(image.DOFade(0, 0.2f));
			sequence.AppendCallback(() =>
			{
				m_Panel.EnemyHpBar.gameObject.SetActive(false);
				m_IsEnemyHpBarAnim = false;
			});
			return;
		}

		m_EnemyHpBarHideTimer = Time.time;
	}

	public void SetRound(int round)
	{
		m_Panel.TxtStage.text = round.ToString();
	}

	public void SetPlayerLife(int life)
	{
		m_Panel.TxtPlayerLife.text = life.ToString();
	}

	public void SetPlayerExp(int currExp,int maxExp)
	{
		string currExpStr = GetExpStr(currExp);
		string maxExpStr = GetExpStr(maxExp);
		m_Panel.TxtExp.text = string.Format("{0}/{1}", currExpStr, maxExpStr);
	}

	public void SetPlayerLevel()
	{
		m_Panel.LevelListGroupView.Update(5);
	}

	private string GetExpStr(int exp)
	{
		string expStr = exp.ToString();
		if (expStr.Length >= 3) return expStr;
		int diff = 3 - expStr.Length;
		for (int i = 0; i < diff; i++)
		{
			expStr = "0" + expStr;
		}

		return expStr;
	}

	private bool m_IsEnemyHpBarAnim = false;
	private float m_EnemyHpBarHideTimer = -1;
	private const float ENEMY_HP_BAR_HIDE = 4f;
	private MainPanel m_Panel = null;
}