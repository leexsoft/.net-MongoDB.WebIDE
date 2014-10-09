using System.Security.Cryptography;
using System;

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
        /// 本地数据库名称
        /// </summary>
        public static readonly string LocalDBName = "local";

        /// <summary>
        /// 索引表名称
        /// </summary>
        public static readonly string IndexTableName = "system.indexes";
        /// <summary>
        /// profile表名称
        /// </summary>
        public static readonly string ProfileTableName = "system.profile";
        /// <summary>
        /// 主服务器日志表名称
        /// </summary>
        public static readonly string OplogTableName = "oplog.$main";
        /// <summary>
        /// 源表名称
        /// </summary>
        public static readonly string SourceTableName = "sources";

        /// <summary>
        /// 随机数的步长
        /// </summary>
        private static readonly int StepLength = 8;
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <returns></returns>
        public static uint GetRandomId()
        {
            var bytes = new byte[StepLength];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}