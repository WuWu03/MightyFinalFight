using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public abstract class BaseExporter : IExporter
    {
        protected int year
        {
            get
            {
                return DateTime.Now.Year;
            }
        }

        protected int month
        {
            get
            {
                return DateTime.Now.Month;
            }
        }

        protected int day
        {
            get
            {
                return DateTime.Now.Day;
            }
        }

        protected int hour
        {
            get
            {
                return DateTime.Now.Hour;
            }
        }

        protected int minute
        {
            get
            {
                return DateTime.Now.Minute;
            }
        }

        public abstract string CopyRef(UIRef[] uiRefs);

        public abstract void Export(UIRef[] uiRefs, UIRefSetting setting);
    }
}