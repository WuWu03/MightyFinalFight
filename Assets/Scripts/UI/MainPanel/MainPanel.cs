/*******************************************************/
/**2020-7-22 19:39**************************************/
/**Create By WuWu***************************************/
/*******************************************************/

using DG.Tweening;
using GameFrameWork;
using GameFrameWork.Event;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel<MainPanelComponent, MainPanelSettings>
{
	protected override void OnInit(object arg)
	{
        m_Component.levelListGroupView.onItemUpdateEvent += OnLevelItemUpdate;
    }

	protected override void OnOpen()
	{
		m_Component.enemyHpBar.SetActiveSelf(false);
		SetPlayerExp(PlayerMgr.instance.exp, PlayerMgr.instance.levelConfigData.exp);
		SetRound(StageMgr.instance.currStageData.StageIndex);
		SetPlayerLife(PlayerMgr.instance.life);
		SetPlayerHP(PlayerMgr.instance.levelConfigData.hpValue, PlayerMgr.instance.levelConfigData.hpValue, PlayerMgr.instance.levelConfigData.hpBarWidth);
		AddEvent(EventDefine.StageEnterStartEvent, OnStageEnterStartEvent);
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
			m_Component.enemyHpBar.gameObject.SetActiveSelf(false);
			m_EnemyHpBarHideTimer = -1;
		}
	}

	protected override void OnClose()
	{

	}

	protected override void OnDestroy()
	{
        m_Component.levelListGroupView.onItemUpdateEvent -= OnLevelItemUpdate;
    }

	private void OnLevelItemUpdate(MainPanelComponent.LevelListItem item)
	{
		int stageIndex = StageMgr.instance.currStageData.StageIndex;
		int playerLevel = PlayerMgr.instance.level;
		item.imgLevel1.gameObject.SetActiveSelf(stageIndex == 1 && playerLevel >= item.id);
		item.imgLevel2.gameObject.SetActiveSelf(stageIndex == 2 && playerLevel >= item.id);
		item.imgLevel3.gameObject.SetActiveSelf(stageIndex == 3 && playerLevel >= item.id);
		item.imgLevel4.gameObject.SetActiveSelf(stageIndex == 4 && playerLevel >= item.id);
		item.imgLevel5.gameObject.SetActiveSelf(stageIndex == 5 && playerLevel >= item.id);
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

		m_Component.enemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		m_Component.enemyHpBar.maxValue = max;
		m_Component.enemyHpBar.value = value;
		m_Component.enemyHpBar.gameObject.SetActiveSelf(true);

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
				m_Component.enemyHpBar.gameObject.SetActiveSelf(false);
				m_IsEnemyHpBarAnim = false;
			});
			return;
		}

		m_EnemyHpBarHideTimer = Time.time;
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
		m_Component.txtExp.text = StringUtil.Append(currExpStr, "/", maxExpStr);
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
        Color color = CommonUtil.HexToRGB(StageMgr.instance.currStageData.StageColor);
		m_Component.playerHpBarImage.color = color;
		m_Component.enemyHpBarImage.color = color;
		m_Component.levelListGroupView.Update(5);
	}

	private bool m_IsEnemyHpBarAnim = false;
	private float m_EnemyHpBarHideTimer = -1;
}