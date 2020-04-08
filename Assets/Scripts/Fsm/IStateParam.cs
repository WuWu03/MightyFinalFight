using FrameWork;

namespace Runtime
{
    interface IStateParam<T> where T:BaseEventArgs
    {
        T StateParam { get; set; }
    }
}
