using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace MongoDB.Defination
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum MongoTreeNodeType
    {
        /// <summary>
        /// 服务器
        /// </summary>
        [Description("服务器")]
        Server = 1,
        /// <summary>
        /// 数据库
        /// </summary>
        [Description("数据库")]
        Database = 2,
        /// <summary>
        /// 数据表
        /// </summary>
        [Description("数据表")]
        Collection = 3,
        /// <summary>
        /// 字段
        /// </summary>
        [Description("字段")]
        Field = 4,
        /// <summary>
        /// 索引
        /// </summary>
        [Description("索引")]
        Index =5,
        /// <summary>
        /// 表信息填充节点
        /// </summary>
        [Description("表信息填充节点")]
        TableFiller = 6,
        /// <summary>
        /// 索引填充节点
        /// </summary>
        [Description("索引填充节点")]
        IndexFiller = 7
    }
}