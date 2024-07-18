/*******************************************************/
/**2024-06-06 16:54****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFrameWork.UI;
using GameFrameWork.Camera;
using GameFrameWork.Pool;
using GameFrameWork.Utilities;
using System;

public class HudPanel : BasePanel
{
    protected override Type componentType
    {
        get
        {
            return typeof(HudPanelComponent);
        }
    }

    protected override Type settingsType
    {
        get
        {
            return typeof(HudPanelSettings);
        }
    }

    protected override void OnInit(BasePanelComponent panelComponent, object[] param)
    {
        m_Component = panelComponent as HudPanelComponent;
    }

	protected override void OnOpen()
	{
        GameObjectPool.instance.AddPool("PlayerDamageText", m_Component.txtPlayerDamageGO);
        GameObjectPool.instance.AddPool("EnemyDamageText", m_Component.txtEnemyDamageGO);
    }

	protected override void OnUpdate()
	{

	}

	protected override void OnClose()
	{

	}

	protected override void OnDestroy()
	{
	}

    public void ShowEnemyDamage(int value, Vector3 pos)
    {
        ShowDamageText("EnemyDamageText", value, pos);
    }

    public void ShowPlayerDamage(int value, Vector3 pos)
    {
        ShowDamageText("PlayerDamageText", value, pos);
    }

    private void ShowDamageText(string textName, int value, Vector3 pos)
    {
        GameObject go = GameObjectPool.instance.Get(textName, transform, LayerName.UI, true);
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

    private HudPanelComponent m_Component = null;


}