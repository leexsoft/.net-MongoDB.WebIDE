using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDB.Model
{
    /// <summary>
    /// 表信息
    /// </summary>
    public class MongoCollection
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表命名空间
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// 全信息
        /// </summary>
        public string FullInfo { get { return string.Format("数据表：{0} | 命名空间：{1}", Name, Namespace); } }
    }
}