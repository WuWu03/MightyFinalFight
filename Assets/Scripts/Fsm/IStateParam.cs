namespace Runtime
{
    interface IStateParam<T> where T:BaseData
    {
        T StateParam { get; set; }
    }
}
