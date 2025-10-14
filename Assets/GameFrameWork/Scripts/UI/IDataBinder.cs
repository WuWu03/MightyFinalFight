namespace GameFrameWork.UI
{
    public interface IDataBinder
    {
        public uint key { get; }
        public void Bind(object call);
        public void UnBind(object call);
        public void UnBindAll();
    }
}