/*******************************************************/
/**2024-06-06 16:54****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Camera;
using GameFrameWork.Pool;
using GameFrameWork.Utils;
using TMPro;

public class HudPanel : BasePanel
{
    protected override void OnInit(object[] param)
    {
        m_Component = GetPanelComponent<HudPanelComponent>();
        GameObjectPoolMgr.instance.AddPool(m_DamageText, m_Component.txtDamageGO);
    }

    protected override void OnOpen()
    {

    }

    protected override void OnUpdate()
    {

    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {
        GameObjectPoolMgr.instance.RemovePool(m_DamageText);
    }

    public void ShowDamageText(HudMgr.DamageType damageType, int value, Vector3 pos)
    {
        GameObject go = GameObjectPoolMgr.instance.Get(m_DamageText, transform, LayerName.UI, true);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        RectTransform textRect = text.GetComponent<RectTransform>();

        text.text = value.ToString();
        text.DOFade(1, 0);
        text.transform.localScale = Vector3.one * 2f;
        text.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBack);

        if (damageType == HudMgr.DamageType.Player)
        {
            text.color = Color.white; // 绿色为玩家伤害
        }
        else
        {
            text.color = Color.red;//红色为敌人伤害 
        }

        Vector3 screenPos = CameraMgr.instance.WorldPosToScreenPos(pos);
        Vector2 uguiPos = CommonUtil.ScreenPosToUGUIPos(screenPos, gameObject.GetComponent<RectTransform>(), UIMgr.instance.uiCamera);
        textRect.localPosition = uguiPos;
        textRect.DOAnchorPos3DY(uguiPos.y + 100f, 2f);
        text.DOFade(0, 2f).OnComplete(() =>
        {
            GameObjectPoolMgr.instance.Put(m_DamageText, go);
        });
    }

    private const string m_DamageText = "DamageText";
    private HudPanelComponent m_Component = null;
}