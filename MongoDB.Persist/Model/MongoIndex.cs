using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDB.Model
{
    /// <summary>
    /// 索引器信息
    /// </summary>
    public class MongoIndex
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 索引名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否唯一索引
        /// </summary>
        public bool Unique { get; set; }
    }
}