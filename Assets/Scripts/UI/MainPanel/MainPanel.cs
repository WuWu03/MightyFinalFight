/*******************************************************/
/**2020-7-22 19:39**************************************/
/**Create By GQY****************************************/
/*******************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.UI;
using GameFrameWork.Utilities;
using GameFrameWork.Pool;
using GameFrameWork.Camera;
using System;
using GameFrameWork.Event;
using GameFrameWork.Resources;
using System.Xml.Linq;

public class MainPanel : BasePanel
{
	public override string panelName { get { return "MainPanel"; } }
	public override float panelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type panelType { get { return UIMgr.Type.Root; } }
	public override UIMgr.Layer panelLayer { get { return UIMgr.Layer.Layer2; } }
	public override UIMgr.CloseMode panelCloseMode { get { return UIMgr.CloseMode.Destroy; } }

    protected override void OnInit(object[] param)
	{
		m_Component = new MainPanelComponent(m_UIRefRoot);
		m_Component.levelListGroupView.Init(m_Component.levelList, m_Component.itemGO, 5);
	}

	protected override void OnOpen()
	{
		m_Component.levelListGroupView.onItemUpdateEvent = OnItemUpdate;
		SetPlayerExp(PlayerMgr.instance.exp, PlayerMgr.instance.levelConfigData.exp);
		SetRound(StageMgr.instance.currStageData.StageIndex);
		SetPlayerLife(PlayerMgr.instance.life);
		SetPlayerHP(PlayerMgr.instance.levelConfigData.hpValue, PlayerMgr.instance.levelConfigData.hpValue, PlayerMgr.instance.levelConfigData.hpBarWidth);

		GameObjectPool.instance.AddPool("PlayerDamageText", m_Component.txtPlayerDamage.gameObject);
		GameObjectPool.instance.AddPool("EnemyDamageText", m_Component.txtEnemyDamage.gameObject);

		AddEvent(EventDefine.StageEnterStartEventId, OnStageEnterStartEvent);
		SetColor();
	}

    private void OnStageEnterStartEvent(object sender, GameEventArgs e)
    {
		SetColor();
    }

    protected override void OnUpdate()
    {
		if (m_EnemyHpBarHideTimer > 0 && Time.time - m_EnemyHpBarHideTimer >= ConstField.EnemyHPBarHideTime)
		{
			m_Component.enemyHpBar.gameObject.SetActive(false);
			m_EnemyHpBarHideTimer = -1;
		}
	}

	protected override void OnClose()
	{
		m_Component.levelListGroupView.onItemUpdateEvent = null;
    }

	protected override void OnDestroy()
	{

	}

	private void OnItemUpdate(MainPanelComponent.LevelListItem item)
	{
		int stageIndex = StageMgr.instance.currStageData.StageIndex;
		int playerLevel = PlayerMgr.instance.level;
		item.imgLevel1.gameObject.SetActive(stageIndex == 1 && playerLevel >= item.id);
		item.imgLevel2.gameObject.SetActive(stageIndex == 2 && playerLevel >= item.id);
		item.imgLevel3.gameObject.SetActive(stageIndex == 3 && playerLevel >= item.id);
		item.imgLevel4.gameObject.SetActive(stageIndex == 4 && playerLevel >= item.id);
		item.imgLevel5.gameObject.SetActive(stageIndex == 5 && playerLevel >= item.id);
	}

	public void SetPlayerHP(int value, int max, float width = 0f)
	{
		if (width != 0)
		{
			m_Component.playerHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		}

		m_Component.playerHpBar.maxValue = max;
		m_Component.playerHpBar.value = value;
	}

	public void SetEnemyHP(int value, int max, float width)
	{
		if (m_IsEnemyHpBarAnim)
		{
			return;
		}

		m_Component.enemyHpBar.gameObject.SetActive(true);
		m_Component.enemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		m_Component.enemyHpBar.maxValue = max;
		m_Component.enemyHpBar.value = value;

		Image image = m_Component.enemyHpBar.GetComponent<Image>();
		image.DOFade(1, 0);

		if (value == 0)
		{
			m_EnemyHpBarHideTimer = -1;
			m_IsEnemyHpBarAnim = true;

			Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < 7; i++)
            {
				sequence.Append(image.DOFade(i % 2, 0.2f));
			}
			sequence.AppendCallback(() =>
			{
				m_Component.enemyHpBar.gameObject.SetActive(false);
				m_IsEnemyHpBarAnim = false;
			});
			return;
		}

		m_EnemyHpBarHideTimer = Time.time;
	}

	public void ShowEnemyDamage(int value,Vector3 pos)
    {
		ShowDamageText("EnemyDamageText", value, pos);
	}

	public void ShowPlayerDamage(int value, Vector3 pos)
	{
		ShowDamageText("PlayerDamageText", value, pos);
	}

	private void ShowDamageText(string textName, int value, Vector3 pos)
	{
		Debug.Log("ÏÔÊ¾ÉËº¦ÎÄ±¾¿ò : " + textName);

		GameObject go = GameObjectPool.instance.Get(textName, transform, "UI", true);
		Text text = go.GetComponent<Text>();
		RectTransform textRect = text.GetComponent<RectTransform>();

		text.text = value.ToString();
		text.DOFade(1, 0);
		text.transform.localScale = Vector3.one * 2f;
		text.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBack);
		Vector3 screenPos = CameraMgr.instance.WorldPosToScreenPos(pos);
		Vector2 uguiPos = CommonUtil.ScreenPosToUGUIPos(screenPos, gameObject.GetComponent<RectTransform>(), UIMgr.instance.uiCamera);
		textRect.localPosition = uguiPos;
		textRect.DOAnchorPos3DY(uguiPos.y + 100f, 2f);
		text.DOFade(0, 2f).OnComplete(() =>
		{
			GameObjectPool.instance.Put(textName, go);
		});
	}

	public void SetRound(int round)
	{
		m_Component.txtStage.text = round.ToString();
	}

	public void SetPlayerLife(int life)
	{
		m_Component.txtPlayerLife.text = life.ToString();
	}

	public void SetPlayerExp(int currExp, int maxExp)
	{
		string currExpStr = GetExpStr(currExp);
		string maxExpStr = GetExpStr(maxExp);
		m_Component.txtExp.text = PathUtil.FormatPath(currExpStr, maxExpStr);
	}

	public void SetPlayerLevel()
	{
		m_Component.levelListGroupView.Update(5);
	}

	private string GetExpStr(int exp)
	{
		return exp.ToString().PadLeft(3, '0');
	}

	private void SetColor()
	{
        m_Component.playerHpBarImage.color = CommonUtil.HexToRGB(StageMgr.instance.currStageData.StageColor);
        m_Component.levelListGroupView.Update(5);
    }

	private bool m_IsEnemyHpBarAnim = false;
	private float m_EnemyHpBarHideTimer = -1;
	private MainPanelComponent m_Component = null;
}