using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskNone : BaseTask
{
    public TaskNone(TaskConfigData data) : base(data)
    {

    }

    public override bool CheckCondition()
    {
        return true;
    }
}
