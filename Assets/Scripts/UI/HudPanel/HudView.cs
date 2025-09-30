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

public class HudView : UIBaseView<HudComponent, HudSettings>
{
    protected override void OnOpen(object arg)
    {
        GameObjectPoolMgr.instance.AddPool(DamageText, component.txtDamageGO);
    }

    protected override void OnShow(object arg)
    {
        
    }
    
    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        
    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {
        GameObjectPoolMgr.instance.RemovePool(DamageText);
    }

    public void ShowDamageText(HudMgr.DamageType damageType, int value, Vector3 pos)
    {
        GameObject go = GameObjectPoolMgr.instance.Get(DamageText, transform, LayerName.UI, true);
        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        RectTransform textRect = text.GetComponent<RectTransform>();

        text.text = value.ToString();
        text.DOFade(1, 0);
        text.transform.localScale = Vector3.one * 2f;
        text.transform.DOScale(1f, 0.3f).SetEase(Ease.InOutBack);

        text.color = damageType == HudMgr.DamageType.Player ? 
            Color.white : // 绿色为玩家伤害
            Color.red; //红色为敌人伤害 

        Vector3 screenPos = CameraMgr.instance.WorldPosToScreenPos(pos);
        Vector2 uguiPos = CommonUtil.ScreenPosToUGUIPos(screenPos, gameObject.GetComponent<RectTransform>(), UIMgr.instance.uiCamera);
        textRect.localPosition = uguiPos;
        textRect.DOAnchorPos3DY(uguiPos.y + 100f, 2f);
        text.DOFade(0, 2f).OnComplete(() =>
        {
            GameObjectPoolMgr.instance.Put(DamageText, go);
        });
    }

    private const string DamageText = "DamageText";
}