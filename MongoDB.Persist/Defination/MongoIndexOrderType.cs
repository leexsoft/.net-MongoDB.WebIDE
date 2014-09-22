using System.ComponentModel;

namespace MongoDB.Defination
{
    /// <summary>
    /// 索引存储的排序方式
    /// </summary>
    public enum MongoIndexOrderType
    {
        /// <summary>
        /// 升序
        /// </summary>
        [Description("升序")]
        Ascending = 1,
        /// <summary>
        /// 降序
        /// </summary>
        [Description("降序")]
        Descending = -1
    }
}