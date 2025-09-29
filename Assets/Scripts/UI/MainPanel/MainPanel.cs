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
        component.levelListGroupView.onItemUpdateEvent += OnLevelItemUpdate;
    }

	protected override void OnOpen()
	{
		component.enemyHpBar.SetActiveSelf(false);
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
			component.enemyHpBar.gameObject.SetActiveSelf(false);
			m_EnemyHpBarHideTimer = -1;
		}
	}

	protected override void OnClose()
	{

	}

	protected override void OnDestroy()
	{
        component.levelListGroupView.onItemUpdateEvent -= OnLevelItemUpdate;
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
			component.playerHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		}

		component.playerHpBar.maxValue = max;
		component.playerHpBar.value = value;
	}

	public void SetEnemyHP(int value, int max, float width)
	{
		if (m_IsEnemyHpBarAnim)
		{
			return;
		}

		component.enemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		component.enemyHpBar.maxValue = max;
		component.enemyHpBar.value = value;
		component.enemyHpBar.gameObject.SetActiveSelf(true);

		Image image = component.enemyHpBar.GetComponent<Image>();
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
				component.enemyHpBar.gameObject.SetActiveSelf(false);
				m_IsEnemyHpBarAnim = false;
			});
			return;
		}

		m_EnemyHpBarHideTimer = Time.time;
	}

	public void SetRound(int round)
	{
		component.txtStage.text = round.ToString();
	}

	public void SetPlayerLife(int life)
	{
		component.txtPlayerLife.text = life.ToString();
	}

	public void SetPlayerExp(int currExp, int maxExp)
	{
		string currExpStr = GetExpStr(currExp);
		string maxExpStr = GetExpStr(maxExp);
		component.txtExp.text = StringUtil.Append(currExpStr, "/", maxExpStr);
	}

	public void SetPlayerLevel()
	{
		component.levelListGroupView.Update(5);
	}

	private string GetExpStr(int exp)
	{
		return exp.ToString().PadLeft(3, '0');
	}

	private void SetColor()
	{
        Color color = CommonUtil.HexToRGB(StageMgr.instance.currStageData.StageColor);
		component.playerHpBarImage.color = color;
		component.enemyHpBarImage.color = color;
		component.levelListGroupView.Update(5);
	}

	private bool m_IsEnemyHpBarAnim = false;
	private float m_EnemyHpBarHideTimer = -1;
}