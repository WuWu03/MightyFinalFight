/*
 * @Desc: Talk 模块 TalkView 界面视图
 * @Date: 2023-11-29 09:28:43
 * @Author: WuWu
 */

using DG.Tweening;
using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Event;
using GameFrameWork.Input;
using GameFrameWork.UI;
using UnityEngine;

public class TalkView : UIBaseView<TalkViewComponent, TalkViewSettings>
{
    private bool m_IsComplete;
    private int m_SelectIndex = -1;
    private TalkConfigData m_ConfigData;
    protected override void OnOpen(object arg)
    {
        component.talkSelectList.onItemUpdateEvent += OnItemUpdateEvent;
        component.talkSelectList.onItemSelectEvent += OnItemSelectEvent;
    }

    protected override void OnShow(object arg)
    {
        int talkId = int.Parse(arg.ToString());
        m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().GetConfigDataById(talkId);
        component.talkSelectList.SetActive(false);
        component.talkSelectList.SelectItem(0);
        PlayTalk();
    }

    protected override void OnUpdate()
    {
        if (GameEntry.inputMgr.GetKeyDown(KeyType.A))
        {
            if (!m_IsComplete)
            {
                component.txtContent.DOComplete();
            }
            else
            {
                if (m_ConfigData.talkSelect is { Length: > 0 })
                {
                    if (m_SelectIndex > -1)
                    {
                        int talkId = m_ConfigData.talkSelect[m_SelectIndex].talkId;
                        m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().GetConfigDataById(talkId);
                        component.talkSelectList.SetActive(false);
                        PlayTalk();
                    }
                }
                else
                {
                    int talkId = m_ConfigData.nextTalkId;
                    m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().GetConfigDataById(talkId);
                    PlayTalk();
                }
            }

            return;
        }

        if (m_IsComplete)
        {
            if (m_ConfigData.talkSelect is { Length: > 0 })
            {
                Vector2 axis = GameEntry.inputMgr.GetAxis(AxisType.LeftAxis);

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
        component.talkSelectList.onItemUpdateEvent -= OnItemUpdateEvent;
        component.talkSelectList.onItemSelectEvent -= OnItemSelectEvent;
    }

    private void PlayTalk()
    {
        if (m_ConfigData == null)
        {
            return;
        }

        string content = GameEntry.localizationMgr.GetLanguageText(m_ConfigData.content);
        component.txtContent.text = string.Empty;
        component.txtContent.DOText(content, m_ConfigData.content.Length * 0.05f).OnComplete(() =>
        {
            m_IsComplete = true;
            component.languageContent.SetLanguageTextKey(m_ConfigData.content);
            
            if (m_ConfigData.talkSelect is { Length: > 0 })
            {
                component.talkSelectList.SetActive(true);
                component.talkSelectList.RefreshItems(m_ConfigData.talkSelect.Length);
                component.talkSelectList.SelectItem(0);
            }
            else
            {
                component.talkSelectList.SetActive(false);

                if (m_ConfigData.nextTalkId == 0)
                {
                    GameEntry.eventMgr.Dispatch(this, EventArg.Create(EventId.TalkEndEvent));
                    CloseSelf();
                }
            }
        });
    }

    private void OnItemUpdateEvent(StaticListItem item)
    {
        if (item is TalkViewComponent.TalkSelectListItem talkSelectItem)
        {
            talkSelectItem.txtSelect.SetLanguageTextKey(m_ConfigData.talkSelect[item.itemIndex].content);
        }
    }

    private void OnItemSelectEvent(StaticListItem item, bool isSelect)
    {
        if (item is TalkViewComponent.TalkSelectListItem talkSelectItem)
        {
            if (isSelect)
            {
                m_SelectIndex = item.itemIndex;
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

        component.talkSelectList.SelectItem(select);
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

        component.talkSelectList.SelectItem(select);
    }
}