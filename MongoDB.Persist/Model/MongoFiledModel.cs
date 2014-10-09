using MongoDB.Bson;
using System;

namespace MongoDB.Model
{
    /// <summary>
    /// 表列名信息
    /// </summary>
    public class MongoFieldModel
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public BsonType Type { get; set; }
    }
}