using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.Editor
{
    public abstract class BaseExporter : IExporter
    {
        protected int Year
        {
            get
            {
                return DateTime.Now.Year;
            }
        }

        protected int Month
        {
            get
            {
                return DateTime.Now.Month;
            }
        }

        protected int Day
        {
            get
            {
                return DateTime.Now.Day;
            }
        }

        protected int Hour
        {
            get
            {
                return DateTime.Now.Hour;
            }
        }

        protected int Minute
        {
            get
            {
                return DateTime.Now.Minute;
            }
        }
        public abstract void Export(UIRef[] uiRefs, UIRefSetting setting);
    }
}