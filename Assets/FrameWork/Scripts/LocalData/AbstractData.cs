
//数据实体基类
namespace GameFrameWork.LocalData
{
    public abstract class AbstractData
    {
        public int ID { get; set; }
        internal abstract void Read(GameDataTableParser parser);
    }
}
