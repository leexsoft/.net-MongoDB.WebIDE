using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Defination;

namespace MongoDB.Model
{
    /// <summary>
    /// 索引器信息
    /// </summary>
    public class MongoIndexModel
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// 索引名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 索引命名空间
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// 索引包含字段
        /// </summary>
        public List<MongoIndexKey> Keys { get; set; }
        /// <summary>
        /// 是否唯一索引
        /// </summary>
        public bool Unique { get; set; }
    }

    public class MongoIndexKey
    {
        /// <summary>
        /// 索引字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 索引字段名称
        /// </summary>
        public MongoIndexOrderType OrderType { get; set; }
    }
}