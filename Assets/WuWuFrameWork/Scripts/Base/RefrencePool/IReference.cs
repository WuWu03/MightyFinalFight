namespace WuWuFramework
{
    public interface IReference
    {
        /// <summary>
        /// 释放引用，将对象返回到引用池中，外部调用（该方法会自动调用Clear方法清理对象数据）
        /// </summary>
        void Release();
        /// <summary>
        /// 清理对象数据，由引用池调用，外部不需要调用该方法
        /// </summary>
        void Clear();
    }
}