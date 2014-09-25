using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Guid ID { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }
    }
}