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

public class MainPanelCtrl:BasePanelCtrl
{
	protected override void OnInit(object[] param)
	{
		m_Panel = Panel as MainPanel;
	}

	protected override void OnLoaded()
	{
	}

	protected override BasePanel GetPanel()
	{
		return new MainPanel();
	}
	protected override void OnOpen()
	{
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

	public void SetPlayerHP(int value,int max)
	{
		m_Panel.PlayerHpBar.value = value;
		m_Panel.PlayerHpBar.maxValue = max;
	}

	public void SetEnemyHP(int value, int max,float width)
	{
		m_Panel.EnemyHpBar.value = value;
		m_Panel.EnemyHpBar.maxValue = max;
		m_Panel.EnemyHpBar.gameObject.SetActive(true);
		m_Panel.EnemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;

		if(value == 0)
		{
			m_EnemyHpBarHideTimer = -1;
			Image image = m_Panel.EnemyHpBar.GetComponent<Image>();
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

	private float m_EnemyHpBarHideTimer = -1;
	private const float ENEMY_HP_BAR_HIDE = 4f;
	private MainPanel m_Panel = null;
}