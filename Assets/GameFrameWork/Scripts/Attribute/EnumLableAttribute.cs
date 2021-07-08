using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class EnumLabelAttribute : PropertyAttribute
    {
        public string label;
        public int[] order = new int[0];
        public EnumLabelAttribute(string label)
        {
            this.label = label;
        }

        public EnumLabelAttribute(string label, params int[] order)
        {
            this.label = label;
            this.order = order;
        }
    }

}