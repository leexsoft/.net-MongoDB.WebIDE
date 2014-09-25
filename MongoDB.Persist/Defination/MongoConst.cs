using System.ComponentModel;

namespace MongoDB.Defination
{
    /// <summary>
    /// 索引存储的排序方式
    /// </summary>
    public static class MongoConst
    {
        /// <summary>
        /// 数据库链接串
        /// </summary>
        public static readonly string ConnString = "mongodb://{0}";

        /// <summary>
        /// 管理数据库名称
        /// </summary>
        public static readonly string AdminDBName = "admin";

        /// <summary>
        /// 索引表名称
        /// </summary>
        public static readonly string IndexTableName = "system.indexes";
    }
}