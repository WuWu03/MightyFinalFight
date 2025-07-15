/*******************************************************/
/**2023-11-29 9:28****************************************/
/**Create By GQY****************************************/
/*******************************************************/
using DG.Tweening;
using GameFrameWork;
using GameFrameWork.ConfigData;
using GameFrameWork.Event;
using GameFrameWork.Input;
using GameFrameWork.Localization;
using GameFrameWork.UI;
using System.Threading.Tasks;
using UnityEngine;

public class TalkPanel : BasePanel<TalkPanelComponent>
{
    protected override void OnInit(object[] param)
	{
		m_TalkId = int.Parse(param[0].ToString());
        m_Component.talkSelectGroupView.Init(m_Component.talkSelect, m_Component.talkSelectItem);
        m_Component.talkSelectGroupView.onItemUpdateEvent += OnItemUpdateEvent;
        m_Component.talkSelectGroupView.onItemSelectEvent += OnItemSelectEvent;
    }

	protected override void OnOpen()
    {

        m_Component.talkSelect.SetActiveSelf(false);
        m_Component.talkSelectGroupView.SelectItem(0);
        PlayTalk();
    }

    protected override void OnUpdate()
	{
		if (InputMgr.instance.GetKeyDown(KeyType.A))
		{
			if (!m_IsComplete)
			{
                m_Component.txtContent.DOComplete();
            }
			else
			{
                TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

                if (talkConfigData.talkSelect != null && talkConfigData.talkSelect.Length > 0)
                {
                    m_TalkId = talkConfigData.talkSelect[m_SelectIndex].talkId;
                    m_Component.talkSelect.SetActiveSelf(false);
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

	protected override void OnClose()
	{
        m_IsComplete = false;
        m_SelectIndex = -1;
        m_TalkId = -1;
    }

	protected override void OnDestroy()
	{

        m_Component.talkSelectGroupView.onItemUpdateEvent -= OnItemUpdateEvent;
        m_Component.talkSelectGroupView.onItemSelectEvent -= OnItemSelectEvent;
    }

	private void PlayTalk()
	{
        m_IsComplete = false;

        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

        if(talkConfigData == null)
        {
            return;
        }

        string content = LocalizationMgr.instance.GetLanguageText(talkConfigData.content);
        m_Component.txtContent.text = string.Empty;

        m_Component.txtContent.DOText(content, talkConfigData.content.Length * 0.05f).OnComplete(async () =>
        {
            m_IsComplete = true;
            m_Component.languageContent.SetLanguageTextKey(talkConfigData.content);

            if (talkConfigData.talkSelect != null && talkConfigData.talkSelect.Length > 0)
            {
                m_Component.talkSelect.SetActiveSelf(true);
                m_Component.talkSelectGroupView.Update(talkConfigData.talkSelect.Length);
                m_Component.talkSelectGroupView.SelectItem(0);
            }
            else
            {
                m_Component.talkSelect.SetActiveSelf(false);

                if (talkConfigData.nextTalkId == 0)
                {
                    await Task.Delay(1000);
                    EventMgr.instance.Dispatch(this, GameEventArgs.Create(EventDefine.TalkEndEvent));
                    CloseSelf();
                }
            }
        });
    }

    private void OnItemUpdateEvent(TalkPanelComponent.TalkSelectItem item)
    {
        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);
        item.languageSelect.SetLanguageTextKey(talkConfigData.talkSelect[item.itemIndex].content);
    }

    private void OnItemSelectEvent(TalkPanelComponent.TalkSelectItem item, bool isSelect)
    {
		if (isSelect)
		{
			m_SelectIndex = item.itemIndex;
        }

		item.selectGO.SetActiveSelf(isSelect);
    }

    private void SelectNext()
    {
        TalkConfigData talkConfigData = ConfigDataSheet.talkConfigDatas.GetConfigDataById(m_TalkId);

		if (talkConfigData.talkSelect == null || talkConfigData.talkSelect.Length < 1)
		{
			return;
		}

		int select = m_SelectIndex + 1;

		if(select >= talkConfigData.talkSelect.Length)
		{
			select = 0;
		}

		m_Component.talkSelectGroupView.SelectItem(select);
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

        m_Component.talkSelectGroupView.SelectItem(select);
    }

    private bool m_IsComplete = false;
	private int m_SelectIndex = -1;
    private int m_TalkId = -1;
}