using System;
using GameFrameWork.UI;

namespace GameFrameWork.Editor
{
    public abstract class BaseUIScriptsExporter : IUIScriptsExporter
    {
        protected int year
        {
            get
            {
                return DateTime.Now.Year;
            }
        }

        protected string month
        {
            get
            {
                return DateTime.Now.Month.ToString().PadLeft(2, '0');
            }
        }

        protected string day
        {
            get
            {
                return DateTime.Now.Day.ToString().PadLeft(2, '0');
            }
        }

        protected string hour
        {
            get
            {
                return DateTime.Now.Hour.ToString().PadLeft(2, '0');
            }
        }

        protected string minute
        {
            get
            {
                return DateTime.Now.Minute.ToString().PadLeft(2, '0');
            }
        }

        protected string second
        {
            get
            {
                return DateTime.Now.Second.ToString().PadLeft(2, '0');
            }
        }

        protected const string Author = "GQY";

        public abstract string CopyRef(UIRef[] uiRefs);

        public abstract void Export(UIRef[] uiRefs, UIRefSetting setting);
    }
}