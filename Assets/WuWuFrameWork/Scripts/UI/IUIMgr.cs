using System;
using WuWuFramework.Pool;

namespace WuWuFramework.UI
{
    /// <summary>
    /// UI层级
    /// </summary>
    public enum UILayer : byte
    {
        Scene,
        Bg,
        MainWindow,
        Window1,
        Window2,
        Tips,
        Talk,
        Guide,
        Message,
        Mask,
        Load,
    }

    /// <summary>
    /// UI销毁模式
    /// </summary>
    public enum UIDestroyMode : byte
    {
        /// <summary>
        /// 常驻场景，关闭达到一定数量后, 会摧毁最先打开的
        /// </summary>
        Always,
        /// <summary>
        /// 关闭时立即销毁
        /// </summary>
        Immediately,
        /// <summary>
        /// 延迟一段时间销毁
        /// </summary>
        Delay,
        /// <summary>
        /// 总是存于场景中，除非主动销毁
        /// </summary>
        Eternal,
    }

    public interface IUIMgr
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        public UIRoot uiRoot { get; }

        /// <summary>
        /// 注入gameObjectPoolMgr依赖
        /// </summary>
        /// <param name="gameObjectPoolMgr"></param>
        public void SetMgr(IGameObjectPoolMgr gameObjectPoolMgr);

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public IUIView Open(string viewName, object arg = null);

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public T Open<T>(object arg = null) where T : class, IUIView, new();

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public IUIView Open(Type viewType, object arg = null);

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class, IUIView, new();

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        public IUIView Get(Type viewType);

        /// <summary>
        /// UI是否打开
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public bool IsOpen(string viewName);

        /// <summary>
        /// UI是否打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsOpen<T>() where T : class, IUIView, new();

        /// <summary>
        /// UI是否打开
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        public bool IsOpen(Type viewType);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="isForceDestroy"></param>
        public void Close(string viewName, bool isForceDestroy = false);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isForceDestroy"></param>
        public void Close<T>(bool isForceDestroy = false) where T : class, IUIView, new();

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="isForceDestroy"></param>
        public void Close(Type viewType, bool isForceDestroy = false);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isForceDestroy"></param>
        /// <param name="checkPopPanel"></param>
        public void Close(IUIView view, bool isForceDestroy = false, bool checkPopPanel = true);
    }
}