/*******************************************************/
/**2020-7-22 19:39**************************************/
/**Create By GQY****************************************/
/*******************************************************/

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.UI;
using GameFrameWork.Utility;
using GameFrameWork.Pool;
using GameFrameWork.Camera;

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

		PoolMgr.Ins.AddPool("PlayerDamageText", m_Component.TxtPlayerDamage.gameObject);
		PoolMgr.Ins.AddPool("EmenyDamageText", m_Component.TxtEnemyDamage.gameObject);
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
		m_Component.LevelListGroupView.OnItemUpdate = null;
	}

	protected override void OnDestroy()
	{

	}

	private void OnItemUpdate(MainPanelComponent.LevelListItem item)
	{
		int stageIndex = StageMgr.Ins.StageIndex;
		int playerLevel = PlayerMgr.Ins.Level;
		item.ImgLevel1.gameObject.SetActive(stageIndex == 1 && playerLevel >= item.Id);
		item.ImgLevel2.gameObject.SetActive(stageIndex == 2 && playerLevel >= item.Id);
		item.ImgLevel3.gameObject.SetActive(stageIndex == 3 && playerLevel >= item.Id);
		item.ImgLevel4.gameObject.SetActive(stageIndex == 4 && playerLevel >= item.Id);
		item.ImgLevel5.gameObject.SetActive(stageIndex == 5 && playerLevel >= item.Id);
	}

	public void SetPlayerHP(int value, int max, float width = 0f)
	{
		if (width != 0)
		{
			m_Component.PlayerHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		}

		m_Component.PlayerHpBar.maxValue = max;
		m_Component.PlayerHpBar.value = value;
	}

	public void SetEnemyHP(int value, int max, float width)
	{
		if (m_IsEnemyHpBarAnim)
		{
			return;
		}

		m_Component.EnemyHpBar.gameObject.SetActive(true);
		m_Component.EnemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
		m_Component.EnemyHpBar.maxValue = max;
		m_Component.EnemyHpBar.value = value;

		Image image = m_Component.EnemyHpBar.GetComponent<Image>();
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
				m_Component.EnemyHpBar.gameObject.SetActive(false);
				m_IsEnemyHpBarAnim = false;
			});
			return;
		}

		m_EnemyHpBarHideTimer = Time.time;
	}

	public void ShowEnemyDamage(int value,Vector3 pos)
    {
		ShowDamageText("EmenyDamageText", value, pos);
	}

	public void ShowPlayerDamage(int value, Vector3 pos)
	{
		ShowDamageText("PlayerDamageText", value, pos);
	}

	private void ShowDamageText(string textName, int value, Vector3 pos)
	{
		GameObject go = PoolMgr.Ins.Spawn(textName, transform, "UI", true);
		Text text = go.GetComponent<Text>();
		RectTransform textRect = text.GetComponent<RectTransform>();

		text.text = value.ToString();
		text.DOFade(1, 0);
		text.transform.localScale = Vector3.one * 2f;
		text.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBack);
		Vector3 screenPos = CameraMgr.Ins.WorldPosToScreenPos(pos);
		Vector2 uguiPos = Util.ScreenPosToUGUIPos(screenPos, gameObject.GetComponent<RectTransform>(), UIMgr.Ins.UICamera);
		textRect.localPosition = uguiPos;
		textRect.DOAnchorPos3DY(uguiPos.y + 100f, 2f);
		text.DOFade(0, 2f).OnComplete(() =>
		{
			PoolMgr.Ins.UnSpawn("EmenyDamageText", go);
		});
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
		m_Component.TxtExp.text = TextUtil.Format("{0}/{1}", currExpStr, maxExpStr);
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