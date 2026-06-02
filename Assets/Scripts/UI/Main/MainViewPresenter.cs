/*
 * @Desc: Main 模块 MainView 界面数据
 * @Date: 2020-07-22 19:39:11
 * @Author: WuWu
 * @Note: 工具生成，请勿修改
 */

using DG.Tweening;
using WuWuFramework;
using WuWuFramework.Event;
using WuWuFramework.UI;
using WuWuFramework.Utils;
using UnityEngine;
using UnityEngine.UI;

public class MainViewPresenter : UIBaseViewPresenter<MainView>
{
    protected override void OnOpen(object arg)
    {
        view.levelList.itemUpdateEvent += OnLevelItemUpdate;
    }

    protected override void OnShow(object arg)
    {
        view.enemyHpBar.SetActiveSelf(false);
        SetPlayerExp(PlayerMgr.instance.exp, PlayerMgr.instance.levelConfigData.exp);
        SetRound(StageMgr.instance.CurrStageData.StageIndex);
        SetPlayerLife(PlayerMgr.instance.life);
        SetPlayerHP(PlayerMgr.instance.levelConfigData.hpValue, PlayerMgr.instance.levelConfigData.hpValue, PlayerMgr.instance.levelConfigData.hpBarWidth);
        GameEntry.eventMgr.Subscribe<StageEnterStartEvent>(OnStageEnterStartEvent).UnSubscribeAllOnDisable(view.gameObject);
        SetColor();
    }

    private void OnStageEnterStartEvent(object sender, StageEnterStartEvent e)
    {
        SetColor();
    }

    protected override void OnUpdate()
    {
        if (m_EnemyHpBarHideTimer > 0 && Time.time - m_EnemyHpBarHideTimer >= ConstField.EnemyHPBarHideTime)
        {
            view.enemyHpBar.gameObject.SetActiveSelf(false);
            m_EnemyHpBarHideTimer = -1;
        }
    }

    protected override void OnHide()
    {

    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {
        view.levelList.itemUpdateEvent -= OnLevelItemUpdate;
    }

    private void OnLevelItemUpdate(BaseListItem item)
    {
        if (item is MainView.LevelListItem levelListItem)
        {
            int stageIndex = StageMgr.instance.CurrStageData.StageIndex;
            int playerLevel = PlayerMgr.instance.level;
            levelListItem.imgLevel1Go.gameObject.SetActiveSelf(stageIndex == 1 && playerLevel >= item.id);
            levelListItem.imgLevel2Go.gameObject.SetActiveSelf(stageIndex == 2 && playerLevel >= item.id);
            levelListItem.imgLevel3Go.gameObject.SetActiveSelf(stageIndex == 3 && playerLevel >= item.id);
            levelListItem.imgLevel4Go.gameObject.SetActiveSelf(stageIndex == 4 && playerLevel >= item.id);
            levelListItem.imgLevel5Go.gameObject.SetActiveSelf(stageIndex == 5 && playerLevel >= item.id);
        }
    }

    public void SetPlayerHP(int value, int max, float width = 0f)
    {
        if (width != 0)
        {
            view.playerHpBar.GetComponent<LayoutElement>().preferredWidth = width;
        }

        view.playerHpBar.maxValue = max;
        view.playerHpBar.value = value;
    }

    public void SetEnemyHP(int value, int max, float width)
    {
        if (m_IsEnemyHpBarAnim)
        {
            return;
        }

        view.enemyHpBar.GetComponent<LayoutElement>().preferredWidth = width;
        view.enemyHpBar.maxValue = max;
        view.enemyHpBar.value = value;
        view.enemyHpBar.gameObject.SetActiveSelf(true);

        Image image = view.enemyHpBar.GetComponent<Image>();
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
                view.enemyHpBar.gameObject.SetActiveSelf(false);
                m_IsEnemyHpBarAnim = false;
            });
            return;
        }

        m_EnemyHpBarHideTimer = Time.time;
    }

    public void SetRound(int round)
    {
        view.txtStage.text = round.ToString();
    }

    public void SetPlayerLife(int life)
    {
        view.txtPlayerLife.text = life.ToString();
    }

    public void SetPlayerExp(int currExp, int maxExp)
    {
        string currExpStr = GetExpStr(currExp);
        string maxExpStr = GetExpStr(maxExp);
        view.txtExp.text = StringUtil.Append(currExpStr, "/", maxExpStr);
    }

    public void SetPlayerLevel()
    {
        view.levelList.SetItemCount(5);
    }

    private string GetExpStr(int exp)
    {
        return exp.ToString().PadLeft(3, '0');
    }

    private void SetColor()
    {
        Color color = CommonUtil.HexToRGB(StageMgr.instance.CurrStageData.StageColor);
        view.playerHpBarImage.color = color;
        view.enemyHpBarImage.color = color;
        view.levelList.SetItemCount(5);
    }

    private bool m_IsEnemyHpBarAnim = false;
    private float m_EnemyHpBarHideTimer = -1;
}