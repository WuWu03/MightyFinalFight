using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameWork.UI
{
    public abstract class BasePanelComponent
    {
        public BasePanelComponent(UIRefRoot root)
        {
            InitComponent(root);
        }

        protected abstract void InitComponent(UIRefRoot root);
    }
}
