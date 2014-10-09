using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDB.Defination
{
    /// <summary>
    /// 字段节点
    /// </summary>
    public class MongoTreeNode
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public uint PID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public MongoTreeNodeType Type { get; set; }
    }
}