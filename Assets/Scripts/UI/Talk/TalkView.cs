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
using GameFrameWork.Localization;
using GameFrameWork.UI;
using UnityEngine;

public class TalkView : UIBaseView<TalkViewComponent, TalkViewSettings>
{
    protected override void OnOpen(object arg)
    {
        m_TalkId = int.Parse(arg.ToString());
        component.talkSelectGroupView.onItemUpdateEvent += OnItemUpdateEvent;
        component.talkSelectGroupView.onItemSelectEvent += OnItemSelectEvent;
    }

    protected override void OnShow(object arg)
    {
        component.talkSelectGroupView.SetActive(false);
        component.talkSelectGroupView.SelectItem(0);
        PlayTalk();
    }

    protected override void OnUpdate()
    {
        if (InputMgr.instance.GetKeyDown(KeyType.A))
        {
            if (!m_IsComplete)
            {
                component.txtContent.DOComplete();
            }
            else
            {
                TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

                if (talkConfigData.talkSelect != null && talkConfigData.talkSelect.Length > 0)
                {
                    m_TalkId = talkConfigData.talkSelect[m_SelectIndex].talkId;
                    component.talkSelectGroupView.SetActive(false);
                }
                else
                {
                    m_TalkId = talkConfigData.nextTalkId;
                }

                PlayTalk();
            }

            return;
        }

        if (m_IsComplete)
        {
            TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

            if (talkConfigData.talkSelect != null && talkConfigData.talkSelect.Length > 0)
            {
                Vector2 axis = InputMgr.instance.GetAxis(AxisType.LeftAxis);

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
        component.talkSelectGroupView.onItemUpdateEvent -= OnItemUpdateEvent;
        component.talkSelectGroupView.onItemSelectEvent -= OnItemSelectEvent;
    }

    private void PlayTalk()
    {
        m_IsComplete = false;

        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

        if (talkConfigData == null)
        {
            return;
        }

        string content = LocalizationMgr.instance.GetLanguageText(talkConfigData.content);
        component.txtContent.text = string.Empty;

        component.txtContent.DOText(content, talkConfigData.content.Length * 0.05f).OnComplete(() =>
        {
            m_IsComplete = true;
            component.languageContent.SetLanguageTextKey(talkConfigData.content);

            if (talkConfigData.talkSelect != null && talkConfigData.talkSelect.Length > 0)
            {
                component.talkSelectGroupView.SetActive(true);
                component.talkSelectGroupView.Update(talkConfigData.talkSelect.Length);
                component.talkSelectGroupView.SelectItem(0);
            }
            else
            {
                component.talkSelectGroupView.SetActive(false);

                if (talkConfigData.nextTalkId == 0)
                {
                    EventMgr.instance.Dispatch(this, EventArg.Create(EventId.TalkEndEvent));
                    CloseSelf();
                }
            }
        });
    }

    private void OnItemUpdateEvent(TalkViewComponent.TalkSelectItem item)
    {
        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);
        item.languageSelect.SetLanguageTextKey(talkConfigData.talkSelect[item.itemIndex].content);
    }

    private void OnItemSelectEvent(TalkViewComponent.TalkSelectItem item, bool isSelect)
    {
        if (isSelect)
        {
            m_SelectIndex = item.itemIndex;
        }

        item.selectGo.SetActiveSelf(isSelect);
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

        component.talkSelectGroupView.SelectItem(select);
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

        component.talkSelectGroupView.SelectItem(select);
    }

    private bool m_IsComplete = false;
    private int m_SelectIndex = -1;
    private int m_TalkId = -1;
}