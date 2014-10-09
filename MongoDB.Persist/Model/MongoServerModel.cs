using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDB.Model
{
    /// <summary>
    /// 服务器信息
    /// </summary>
    public class MongoServerModel
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string Name { get { return string.Format("{0}:{1}", IP, Port); } }
        /// <summary>
        /// 总容量
        /// </summary>
        public double TotalSize { get; set; }
        /// <summary>
        /// 服务器状态
        /// </summary>
        public bool IsOK { get; set; }
        /// <summary>
        /// 全信息
        /// </summary>
        public string FullInfo { get { return string.Format("服务器：{0} | 总容量：{1} | 状态：{2}", Name, TotalSize, IsOK ? "良好" : "无法访问"); } }
    }
}