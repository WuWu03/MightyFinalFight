/*******************************************************/
/**2020-7-22 19:39****************************************/
/**Create By GQY****************************************/
/**工具生成，请勿修改************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;

public class MainPanel : BasePanel
{
	public override string PanelName { get { return "MainPanel"; } }
	public override float PanelUnLoadTime { get { return 0f; } }
	public override UIMgr.Type PanelType { get { return UIMgr.Type.Root; } }
	public override UIMgr.Layer PanelLayer { get { return UIMgr.Layer.MainPanel; } }
	public override UIMgr.CloseMode PanelCloseMode { get { return UIMgr.CloseMode.Eternal; } }

    protected override void OnInit(object[] param)
	{
		m_Component = new MainPanelComponent(UIRefRoot);
		m_Component.LevelListGroupView.Init(m_Component.LevelList, m_Component.ItemGO, 5);
	}

    protected override void OnOpen()
	{
		m_Component.LevelListGroupView.OnItemUpdate = OnItemUpdate;
		m_Component.LevelListGroupView.Update(5);
		SetPlayerExp(PlayerMgr.Ins.EXP, PlayerMgr.Ins.LevelData.EXP);
		SetRound(StageMgr.Ins.StageIndex);
		SetPlayerLife(PlayerMgr.Ins.Life);
		SetPlayerHP(PlayerMgr.Ins.LevelData.Health, PlayerMgr.Ins.LevelData.Health, PlayerMgr.Ins.LevelData.HPBarWidth);
	}

    protected override void OnUpdate()
    {
		if (m_EnemyHpBarHideTimer > 0 && Time.time - m_EnemyHpBarHideTimer >= ENEMY_HP_BAR_HIDE)
		{
			m_Component.EnemyHpBar.gameObject.SetActive(false);
			m_EnemyHpBarHideTimer = -1;
		}
	}

	protected override void OnClose()
	{

	}

	protected override void OnDestroy()
	{

	}

	private void OnItemUpdate(MainPanelComponent.LevelListItem obj)
	{
		int stageIndex = StageMgr.Ins.StageIndex;
		int playerLevel = PlayerMgr.Ins.Level;
		obj.ImgLevel1.gameObject.SetActive(stageIndex == 1 && playerLevel >= obj.Index);
		obj.ImgLevel2.gameObject.SetActive(stageIndex == 2 && playerLevel >= obj.Index);
		obj.ImgLevel3.gameObject.SetActive(stageIndex == 3 && playerLevel >= obj.Index);
		obj.ImgLevel4.gameObject.SetActive(stageIndex == 4 && playerLevel >= obj.Index);
		obj.ImgLevel5.gameObject.SetActive(stageIndex == 5 && playerLevel >= obj.Index);
	}

	public void SetPlayerHP(int value, int max, float width = 0f)
	{
		if (width != 0)
			m_Component.PlayerHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		m_Component.PlayerHpBar.maxValue = max;
		m_Component.PlayerHpBar.value = value;
	}

	public void SetEnemyHP(int value, int max, float width)
	{
		if (m_IsEnemyHpBarAnim) return;

		m_Component.EnemyHpBar.value = value;
		m_Component.EnemyHpBar.maxValue = max;
		m_Component.EnemyHpBar.gameObject.SetActive(true);
		m_Component.EnemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		Image image = m_Component.EnemyHpBar.GetComponent<Image>();
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
				m_Component.EnemyHpBar.gameObject.SetActive(false);
				m_IsEnemyHpBarAnim = false;
			});
			return;
		}

		m_EnemyHpBarHideTimer = Time.time;
	}

	public void SetRound(int round)
	{
		m_Component.TxtStage.text = round.ToString();
	}

	public void SetPlayerLife(int life)
	{
		m_Component.TxtPlayerLife.text = life.ToString();
	}

	public void SetPlayerExp(int currExp, int maxExp)
	{
		string currExpStr = GetExpStr(currExp);
		string maxExpStr = GetExpStr(maxExp);
		m_Component.TxtExp.text = string.Format("{0}/{1}", currExpStr, maxExpStr);
	}

	public void SetPlayerLevel()
	{
		m_Component.LevelListGroupView.Update(5);
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
	private MainPanelComponent m_Component = null;
}