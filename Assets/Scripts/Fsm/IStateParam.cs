using FrameWork;

interface IStateParam<T> where T : BaseEventArgs
{
    T StateParam { get; set; }
}
