using GameFrameWork.Event;

namespace GameFrameWork.UI
{
    public interface IDataBinder<T>
    {
        public uint key { get; }
        public T value
        {
            get;
            set;
        }

        public void Bind(GameFrameWorkAction<T> callback);
        public void UnBind(GameFrameWorkAction<T> callback);
        public void UnBindAll();
    }
}