/*
 * @Desc: Talk 模块 TalkView 界面视图
 * @Date: 2023-11-29 09:28:43
 * @Author: GQY
 */

using DG.Tweening;
using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Event;
using GameFrameWork.Input;
using GameFrameWork.UI;
using UnityEngine;

public class TalkPresenter : UIBasePresenter<TalkView, TalkViewSettings>
{
    private bool m_IsComplete;
    private int m_SelectIndex = -1;
    private TalkConfigData m_ConfigData;
    protected override void OnOpen(object arg)
    {
        view.talkSelectList.itemUpdateEvent += OnItemUpdateEvent;
        view.talkSelectList.itemSelectEvent += OnItemSelectEvent;
    }

    protected override void OnShow(object arg)
    {
        int talkId = int.Parse(arg.ToString());
        m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().GetConfigDataById(talkId);
        view.talkSelectList.SetActiveSelf(false);
        view.talkSelectList.SelectItem(0);
        PlayTalk();
    }

    protected override void OnUpdate()
    {
        if (GameEntry.inputMgr.GetKeyDown(KeyType.A))
        {
            if (!m_IsComplete)
            {
                view.txtContent.DOComplete();
            }
            else
            {
                if (m_ConfigData.talkSelect is { Length: > 0 })
                {
                    if (m_SelectIndex > -1)
                    {
                        int talkId = m_ConfigData.talkSelect[m_SelectIndex].talkId;
                        m_ConfigData = GameEntry.configDataMgr.Get<TalkConfigData>().GetConfigDataById(talkId);
                        view.talkSelectList.SetActiveSelf(false);
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
        view.talkSelectList.itemUpdateEvent -= OnItemUpdateEvent;
        view.talkSelectList.itemSelectEvent -= OnItemSelectEvent;
    }

    private void PlayTalk()
    {
        if (m_ConfigData == null)
        {
            return;
        }

        string content = GameEntry.localizationMgr.GetLanguageText(m_ConfigData.content);
        view.txtContent.text = string.Empty;
        view.txtContent.DOText(content, m_ConfigData.content.Length * 0.05f).OnComplete(() =>
        {
            m_IsComplete = true;
            view.languageContent.SetLanguageTextKey(m_ConfigData.content);

            if (m_ConfigData.talkSelect is { Length: > 0 })
            {
                view.talkSelectList.SetActiveSelf(true);
                view.talkSelectList.SetItemCount(m_ConfigData.talkSelect.Length);
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
        });
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