/*
 * @Desc: Hud 模块 HudView 界面视图
 * @Date: 2024-06-06 16:54:04
 * @Author: GQY
 */

using UnityEngine;
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.UI;
using GameFrameWork.Utils;
using TMPro;

public class HudPresenter : UIBasePresenter<HudView, HudViewSettings>
{
    private const string DamageText = "DamageText";
    
    protected override void OnOpen(object arg)
    {
        GameEntry.gameObjectPoolMgr.AddPool(DamageText, view.txtDamageGo);
    }

    protected override void OnShow(object arg)
    {
        
    }
    
    protected override void OnUpdate()
    {
        if (HudMgr.instance.hudArgs.Count > 0)
        {
            ShowDamageText(HudMgr.instance.hudArgs.Dequeue());
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
        GameEntry.gameObjectPoolMgr.RemovePool(DamageText);
    }

    private void ShowDamageText(HudMgr.HudArg arg)
    {
        HudMgr.DamageType damageType = arg.damageType;
        int value = arg.value;
        Vector3 pos = arg.pos;
        GameObject go = GameEntry.gameObjectPoolMgr.Get(DamageText, transform, LayerName.UI, true);
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
        Vector2 uguiPos = CommonUtil.ScreenPosToUGUIPos(screenPos, gameObject.GetComponent<RectTransform>(), GameFrameWorkMgr.GetModule<IUIMgr>().uiCamera);
        textRect.localPosition = uguiPos;
        textRect.DOAnchorPos3DY(uguiPos.y + 100f, 2f);
        text.DOFade(0, 2f).OnComplete(() =>
        {
            GameEntry.gameObjectPoolMgr.Put(DamageText, go);
        });
    }
}