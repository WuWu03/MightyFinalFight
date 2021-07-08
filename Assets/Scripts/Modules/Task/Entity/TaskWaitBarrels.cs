using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskWaitBarrels : BaseTask
{
    public TaskWaitBarrels(TaskConfigData data) : base(data) { }
    public override bool CheckCondition()
    {
        return false;
    }

    public override void Enter()
    {
       
    }

    public override void Update()
    {

    }
}
