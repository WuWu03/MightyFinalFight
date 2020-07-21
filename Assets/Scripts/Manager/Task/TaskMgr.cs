using FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMgr : BaseMgr<TaskMgr>
{
    private void Awake()
    {
        m_CurrTaskList = new List<BaseTask>();
        m_CurrTaskIndex = 0;
    }

    public void AcceptTask(int id)
    {
        //m_CurrTaskList.Add(new TaskState() { ID = id, Complete = false });
    }

    private void Update()
    {
        
    }

    private int m_CurrTaskIndex = 0;
    private List<BaseTask> m_CurrTaskList = null;
}
