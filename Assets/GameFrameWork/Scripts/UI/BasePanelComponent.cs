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

        public virtual void Clear()
        {

        }

        protected abstract void InitComponent(UIRefRoot root);
    }
}
