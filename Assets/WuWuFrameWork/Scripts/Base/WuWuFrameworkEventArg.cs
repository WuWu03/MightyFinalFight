using System;

namespace WuWuFramework
{
    public abstract class WuWuFrameworkEventArg : EventArgs, IReference
    {
        public void Release()
        {
            ReferencePool.Release(this);
        }

        public abstract void Clear();

        public object Clone()
        {
            IReference target = ReferencePool.Acquire(this.GetType());
            Copy(target);
            return target;
        }

        protected virtual void Copy(IReference target)
        {

        }
    }
}