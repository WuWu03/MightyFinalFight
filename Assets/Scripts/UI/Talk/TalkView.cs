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
    private int m_TalkId = -1;
    private int m_CurrTalkId = -1;
    
    protected override void OnOpen(object arg)
    {
        m_TalkId = int.Parse(arg.ToString());
        component.talkSelectList.onItemUpdateEvent += OnItemUpdateEvent;
        component.talkSelectList.onItemSelectEvent += OnItemSelectEvent;
    }

    protected override void OnShow(object arg)
    {
        component.talkSelectList.SetActive(false);
        component.talkSelectList.SelectItem(0);
        PlayTalk(m_TalkId);
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
                TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);
                if (talkConfigData.talkSelect is { Length: > 0 })
                {
                    if (m_SelectIndex > -1)
                    {
                        m_TalkId = talkConfigData.talkSelect[m_SelectIndex].talkId;
                        component.talkSelectList.SetActive(false);
                    }
                }
                else
                {
                    m_TalkId = talkConfigData.nextTalkId;
                }

                PlayTalk(m_TalkId);
            }

            return;
        }

        if (m_IsComplete)
        {
            TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

            if (talkConfigData.talkSelect is { Length: > 0 })
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
        
    }

    protected override void OnClose()
    {
        m_IsComplete = false;
        m_SelectIndex = -1;
        m_TalkId = -1;
    }

    protected override void OnDestroy()
    {
        component.talkSelectList.onItemUpdateEvent -= OnItemUpdateEvent;
        component.talkSelectList.onItemSelectEvent -= OnItemSelectEvent;
    }

    private void PlayTalk(int talkId)
    {
        if (m_CurrTalkId == talkId)
        {
            return;
        }

        m_CurrTalkId = talkId;
        m_IsComplete = false;
        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(talkId);

        if (talkConfigData == null)
        {
            return;
        }

        string content = GameEntry.localizationMgr.GetLanguageText(talkConfigData.content);
        component.txtContent.text = string.Empty;
        component.txtContent.DOText(content, talkConfigData.content.Length * 0.05f).OnComplete(() =>
        {
            m_IsComplete = true;
            component.languageContent.SetLanguageTextKey(talkConfigData.content);
            
            if (talkConfigData.talkSelect is { Length: > 0 })
            {
                component.talkSelectList.SetActive(true);
                component.talkSelectList.RefreshItems(talkConfigData.talkSelect.Length);
                component.talkSelectList.SelectItem(0);
            }
            else
            {
                component.talkSelectList.SetActive(false);

                if (talkConfigData.nextTalkId == 0)
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
            TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);
            talkSelectItem.txtSelect.SetLanguageTextKey(talkConfigData.talkSelect[item.itemIndex].content);
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
        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

        if (talkConfigData.talkSelect == null || talkConfigData.talkSelect.Length < 1)
        {
            return;
        }

        int select = m_SelectIndex + 1;

        if (select >= talkConfigData.talkSelect.Length)
        {
            select = 0;
        }

        component.talkSelectList.SelectItem(select);
    }

    private void SelectPrevious()
    {
        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

        if (talkConfigData.talkSelect == null || talkConfigData.talkSelect.Length < 1)
        {
            return;
        }

        int select = m_SelectIndex - 1;

        if (select < 0)
        {
            select = talkConfigData.talkSelect.Length - 1;
        }

        component.talkSelectList.SelectItem(select);
    }
}