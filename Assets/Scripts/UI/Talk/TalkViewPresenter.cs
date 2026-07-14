/*
 * @Desc: Talk 模块 TalkView 视图展示器
 * @Date: 2023-11-29 09:28:43
 * @Author: WuWu
 */

using DG.Tweening;
using WuWuFramework;
using WuWuFramework.ConfigData;
using WuWuFramework.Input;
using WuWuFramework.UI;
using UnityEngine;

public class TalkViewPresenter : UIBaseViewPresenter<TalkView>
{
    private bool m_IsComplete;
    private int m_SelectIndex = -1;
    private TalkConfigData m_ConfigData;

    protected override void OnOpen(object arg)
    {
        view.talkSelectList.itemUpdateEvent += OnItemUpdateEvent;
        view.talkSelectList.itemSelectEvent += OnItemSelectEvent;
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.A, InputEventCallType.Performed, ConfirmTalk);
        GameEntry.inputMgr.keyBoardInputController.AddInputEvent(KeyboardInputKey.LeftAxis, InputEventCallType.Performed, SelectTalk);
    }

    protected override void OnShow(object arg)
    {
        Debug.Log("开启对话=====================");
        int talkId = int.Parse(arg.ToString());
        m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().Get(talkId);
        view.talkSelectList.SetActiveSelf(false);
        view.talkSelectList.SelectItem(0);
        PlayTalk();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnHide()
    {
        m_IsComplete = false;
        m_SelectIndex = -1;
        m_ConfigData = null;
    }

    protected override void OnClose()
    {

    }

    protected override void OnDestroy()
    {
        view.talkSelectList.itemUpdateEvent -= OnItemUpdateEvent;
        view.talkSelectList.itemSelectEvent -= OnItemSelectEvent;
        GameEntry.inputMgr.keyBoardInputController.RemoveInputEvent(KeyboardInputKey.A, InputEventCallType.Performed, ConfirmTalk);
        GameEntry.inputMgr.keyBoardInputController.RemoveInputEvent(KeyboardInputKey.LeftAxis, InputEventCallType.Performed, SelectTalk);
    }

    private void ConfirmTalk()
    {
        if (!m_IsComplete)
        {
            view.txtContent.DOKill();
            OnTalkAnimComplete();
        }
        else
        {
            if (m_ConfigData.talkSelect is { Length: > 0 })
            {
                if (m_SelectIndex > -1)
                {
                    int talkId = m_ConfigData.talkSelect[m_SelectIndex].talkId;
                    m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().Get(talkId);
                    view.talkSelectList.SetActiveSelf(false);
                    PlayTalk();
                }
            }
            else
            {
                int talkId = m_ConfigData.nextTalkId;
                m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().Get(talkId);
                PlayTalk();
            }
        }
    }

    private void SelectTalk(Vector2 axis)
    {
        if (m_IsComplete)
        {
            if (m_ConfigData.talkSelect is { Length: > 0 })
            {
                if (axis.x > 0)
                {
                    SelectNext();
                }
                else if (axis.x < 0)
                {
                    SelectPrevious();
                }
            }
        }
    }

    private void PlayTalk()
    {
        if (m_ConfigData == null)
        {
            return;
        }

        m_IsComplete = false;
        string content = GameEntry.localizationMgr.GetLanguageText(m_ConfigData.content);
        view.txtContent.text = string.Empty;
        view.txtContent.DOText(content, m_ConfigData.content.Length * 0.05f).OnComplete(OnTalkAnimComplete);
    }

    private void OnTalkAnimComplete()
    {
        m_IsComplete = true;
        view.languageContent.SetLanguageTextKey(m_ConfigData.content);

        if (m_ConfigData.talkSelect is { Length: > 0 })
        {
            view.talkSelectList.SetActiveSelf(true);
            view.talkSelectList.SetItemCount(m_ConfigData.talkSelect.Length, true);
            view.talkSelectList.SelectItem(0);
        }
        else
        {
            view.talkSelectList.SetActiveSelf(false);

            if (m_ConfigData.nextTalkId == 0)
            {
                GameEntry.eventMgr.Dispatch(this, new TalkEndEvent());
                CloseSelf();
            }
        }
    }

    private void OnItemUpdateEvent(BaseListItem item)
    {
        if (item is TalkView.TalkSelectListItem talkSelectItem)
        {
            talkSelectItem.txtSelect.SetLanguageTextKey(m_ConfigData.talkSelect[item.index].content);
        }
    }

    private void OnItemSelectEvent(BaseListItem item, bool isSelect)
    {
        if (item is TalkView.TalkSelectListItem talkSelectItem)
        {
            if (isSelect)
            {
                m_SelectIndex = item.index;
            }

            talkSelectItem.selectGo.SetActiveSelf(isSelect);
        }
    }

    private void SelectNext()
    {
        if (m_ConfigData.talkSelect == null || m_ConfigData.talkSelect.Length < 1)
        {
            return;
        }

        int select = m_SelectIndex + 1;

        if (select >= m_ConfigData.talkSelect.Length)
        {
            select = 0;
        }

        view.talkSelectList.SelectItem(select);
    }

    private void SelectPrevious()
    {
        if (m_ConfigData.talkSelect == null || m_ConfigData.talkSelect.Length < 1)
        {
            return;
        }

        int select = m_SelectIndex - 1;

        if (select < 0)
        {
            select = m_ConfigData.talkSelect.Length - 1;
        }

        view.talkSelectList.SelectItem(select);
    }
}